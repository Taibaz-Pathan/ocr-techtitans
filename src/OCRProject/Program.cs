using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using OCRProject.ImageProcessing;
using OCRProject.ModelComparison;
using OCRProject.Utils;
using OCRProject.TesseractProcessor;
using ModelComparison;

class Program
{
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" };

    static void Main()
    {
        var logger = new Logger();
        var embeddingGenerator = new EmbeddingGeneratorService();

        try
        {
            Console.WriteLine("Starting Image Processing...");
            logger.LogInfo("Starting Image Processing...");

            var config = new ConfigLoader();
            string inputFolder = config.InputFolder;
            string outputFolderImage = config.OutputImageFolder;
            string outputFolderText = config.ExtractedTextFolder;
            string comparisonResults = config.ComparisionFolder;

            logger.LogInfo("Configuration loaded successfully.");

            var directoryCleaner = new DirectoryCleaner(logger);
            directoryCleaner.CleanDirectory(outputFolderImage);
            directoryCleaner.CleanDirectory(outputFolderText);
            directoryCleaner.CleanDirectory(comparisonResults);

            var fileCreator = new CreateFiles();
            string fileNameExtracted = $"ProcessedFile_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string createdFilePath = fileCreator.CreateTextFile(outputFolderText, fileNameExtracted);

            if (string.IsNullOrEmpty(createdFilePath))
            {
                logger.LogError("Failed to create text file.");
                Console.WriteLine("Failed to create text file.");
                Environment.Exit(1);
            }

            logger.LogInfo($"Text file created successfully: {createdFilePath}");

            var fileWriter = new FileWriter();
            var timeTracker = new ProcessingTimeTracker(comparisonResults);
            var memoryTracker = new ProcessingMemoryTracker(comparisonResults);
            var similarityCalculator = new CosineSimilarityCalculator(comparisonResults);

            var imageFiles = Directory.GetFiles(inputFolder, "*.*")
                .Where(file => AllowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToArray();

            if (imageFiles.Length == 0)
            {
                logger.LogWarning("No images found in the input folder.");
                Console.WriteLine("No images found in the input folder.");
                Environment.Exit(0);
            }

            logger.LogInfo($"Processing {imageFiles.Length} images...");

            foreach (var inputFilePath in imageFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    string fileExtension = Path.GetExtension(inputFilePath).ToLower();
                    using var inputImage = Image.Load<Rgba32>(inputFilePath);
                    using var resizedImage = ResizeImage(inputImage, 2000, 2000);

                    var transformations = new Dictionary<string, Func<Image<Rgba32>, Image<Rgba32>>>()
                    {
                        { "Grayscale", img => img.CloneAs<L8>().CloneAs<Rgba32>() },
                        { "GlobalThreshold", img => ApplyGlobalThreshold(img, 1) },
                        { "Shifted", img => ApplyShiftImage(img, 5, 5) },
                        { "SaturationAdjusted", img => ApplySaturationAdjustment(img, 1.2f) },
                        { "Deskewed", img => ApplyDeskew(img) }
                    };

                    var extractedTexts = new Dictionary<string, string>();

                    foreach (var (modelName, transform) in transformations)
                    {
                        Image<Rgba32>? processedImage = null;

                        timeTracker.StartTimer();
                        memoryTracker.MeasureMemoryUsage(fileName, modelName, () =>
                        {
                            processedImage = transform(resizedImage.Clone());
                        });
                        timeTracker.StopAndRecord(fileName, modelName);

                        if (processedImage == null)
                        {
                            throw new InvalidOperationException("Image transformation returned null.");
                        }

                        string outputPath = Path.Combine(outputFolderImage, $"{fileName}_{modelName}{fileExtension}");
                        processedImage.Save(outputPath, GetImageEncoder(fileExtension));
                        logger.LogInfo($"Saved processed image: {outputPath}");

                        string extractedText = TesseractProcessor.ExtractTextFromImage(processedImage, modelName, createdFilePath, fileWriter);
                        extractedTexts[modelName] = extractedText;
                    }

                    var embeddings = embeddingGenerator.GenerateEmbeddingsForModels(extractedTexts);
                    similarityCalculator.ComputeAndSaveReport(embeddings);
                    logger.LogInfo($"Successfully processed image: {fileName}{fileExtension}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing {inputFilePath}: {ex.Message}");
                    Console.WriteLine($"Error processing {inputFilePath}: {ex.Message}");
                }
            }

            timeTracker.GenerateExcelReport();
            memoryTracker.AppendMemoryUsageToExcel();

            // Run final model evaluation and ranking
            string similarityFilePath = Path.Combine(comparisonResults, "CosineSimilarity.xlsx");
            string performanceFilePath = Path.Combine(comparisonResults, "ProcessingResults.xlsx");

            if (File.Exists(similarityFilePath) && File.Exists(performanceFilePath))
            {
                var evaluator = new PreprocessingModelEvaluator(similarityFilePath, performanceFilePath);
                evaluator.EvaluateAndReportBestModels();
            }
            else
            {
                logger.LogWarning("Similarity or Performance file missing. Skipping model evaluation.");
            }


            logger.LogInfo("Processing completed successfully.");
            Console.WriteLine("Processing completed.");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            logger.LogError($"Critical error in process: {ex.Message}");
            Console.WriteLine($"Error in Process: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static Image<Rgba32> ApplyGlobalThreshold(Image<Rgba32> image, byte threshold)
    {
        var grayImage = image.CloneAs<L8>();
        grayImage.Mutate(x => x.BinaryThreshold(threshold));
        return grayImage.CloneAs<Rgba32>();
    }

    private static Image<Rgba32> ApplyShiftImage(Image<Rgba32> image, int shiftX, int shiftY)
    {
        image.Mutate(x => x.Transform(new AffineTransformBuilder().AppendTranslation(new SixLabors.ImageSharp.PointF(shiftX, shiftY))));
        return image;
    }

    private static Image<Rgba32> ApplySaturationAdjustment(Image<Rgba32> image, float saturation)
    {
        image.Mutate(x => x.Saturate(saturation));
        return image;
    }

    private static Image<Rgba32> ApplyDeskew(Image<Rgba32> image)
    {
        // Deskew logic placeholder
        return image;
    }

    private static Image<Rgba32> ResizeImage(Image<Rgba32> image, int maxWidth, int maxHeight)
    {
        int newWidth = image.Width;
        int newHeight = image.Height;

        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);
            newWidth = (int)(image.Width * ratio);
            newHeight = (int)(image.Height * ratio);
        }

        image.Mutate(x => x.Resize(newWidth, newHeight));
        return image;
    }

    private static IImageEncoder GetImageEncoder(string extension) => extension.ToLower() switch
    {
        ".bmp" => new BmpEncoder(),
        ".gif" => new GifEncoder(),
        ".jpg" => new JpegEncoder(),
        ".jpeg" => new JpegEncoder(),
        ".png" => new PngEncoder(),
        ".tiff" => new TiffEncoder(),
        _ => new PngEncoder()
    };
}
