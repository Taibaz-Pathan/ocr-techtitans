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
using ModelComparison;
using OCRProject.Utils;
using OCRProject.ModelComparison;
using OCRProject.TesseractProcessor;

class Program
{
    static void Main(string[] args)
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
            var similarityCalculator = new CosineSimilarityCalculator(comparisonResults);

            var imageFiles = Directory.GetFiles(inputFolder, "*.*")
                .Where(file => new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" }
                .Contains(Path.GetExtension(file).ToLower()))
                .ToArray();

            if (imageFiles.Length == 0)
            {
                logger.LogWarning("No images found in the input folder.");
                Console.WriteLine("No images found in the input folder.");
                Environment.Exit(0); // Exit cleanly if no images are found.
            }

            logger.LogInfo($"Processing {imageFiles.Length} images...");

            foreach (var inputFilePath in imageFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    string fileExtension = Path.GetExtension(inputFilePath).ToLower();
                    using var inputImage = SixLabors.ImageSharp.Image.Load<Rgba32>(inputFilePath);
                    using var resizedImage = ResizeImage(inputImage, 2000, 2000);

                    var transformations = new Dictionary<string, Func<Image<Rgba32>, Image<Rgba32>>>
                    {
                        { "Grayscale", img => img.CloneAs<L8>().CloneAs<Rgba32>() },
                        { "GlobalThreshold", img => ApplyGlobalThreshold(img, 1) },
                        { "Shifted", img => ApplyShiftImage(img, 5, 5) },
                        { "SaturationAdjusted", img => ApplySaturationAdjustment(img, 1.2f) }
                        // { "Deskewed", img => ApplyDeskew(img) }
                    };

                    var extractedTexts = new Dictionary<string, string>();

                    foreach (var (modelName, transform) in transformations)
                    {
                        timeTracker.StartTimer();
                        using var processedImage = transform(resizedImage.Clone()); // Clone to avoid modifying original

                        string outputPath = Path.Combine(outputFolderImage, $"{fileName}_{modelName}{fileExtension}");
                        processedImage.Save(outputPath, GetImageEncoder(fileExtension));

                        logger.LogInfo($"Saved processed image: {outputPath}");

                        string extractedText = TesseractProcessor.ExtractTextFromImage(processedImage, createdFilePath, fileWriter);
                        extractedTexts[modelName] = extractedText;

                        timeTracker.StopAndRecord(fileName, modelName);
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
            logger.LogInfo("Processing completed successfully.");
            Console.WriteLine("Processing completed.");
            Environment.Exit(0); // Ensure process exits cleanly
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

    private static Image<Rgba32> ApplySaturationAdjustment(Image<Rgba32> image, float saturationFactor)
    {
        image.Mutate(x => x.Saturate(saturationFactor));
        return image;
    }

    // private static Image<Rgba32> ApplyDeskew(Image<Rgba32> image)
    // {
    //     return new Deskew().Apply(image);
    // }

    private static Image<Rgba32> ResizeImage(Image<Rgba32> image, int maxWidth, int maxHeight)
    {
        var ratio = Math.Min((double)maxWidth / image.Width, (double)maxHeight / image.Height);
        image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Size((int)(image.Width * ratio), (int)(image.Height * ratio))));
        return image;
    }

    private static IImageEncoder GetImageEncoder(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => new JpegEncoder(),
            ".bmp" => new BmpEncoder(),
            ".gif" => new GifEncoder(),
            ".tiff" => new TiffEncoder(),
            _ => new PngEncoder(),
        };
    }
}
