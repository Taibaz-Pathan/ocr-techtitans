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
using OCRProject.ModelComparision;
using OCRProject.Utils;
using OCRProject.TesseractProcessor;
using OCRProject.ModelComparison;

class Program
{
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" };

    static async Task Main()
    {
        var logger = new Logger();
        var embeddingGenerator = new EmbeddingGeneratorService();  // Correct reference to EmbeddingGeneratorService

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

            // Clean directories
            var directoryCleaner = new DirectoryCleaner(logger);
            directoryCleaner.CleanDirectory(outputFolderImage);
            directoryCleaner.CleanDirectory(outputFolderText);
            directoryCleaner.CleanDirectory(comparisonResults);

            // Create file for extracted text
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

                    // Define image transformations
                    var transformations = new Dictionary<string, Func<Image<Rgba32>, Image<Rgba32>>>()
                    {
                        { "Grayscale", img => new ConvertToGrayscale().Apply(img).CloneAs<Rgba32>() }, // Grayscale conversion using ConvertToGrayscale
                        { "GlobalThreshold", img => new GlobalThresholding(128).ApplyThreshold(img) },  // Global Thresholding using GlobalThresholding class
                        //{ "Shifted", img => ApplyShift(img, resizedImage, 0, 0) }, // Shift image using custom logic in Program.cs
                        { "SaturationAdjusted", img => ApplySaturationAdjustment(img, 1.2f) }, // Apply saturation adjustment directly
                        { "Deskewed", img => new Deskew().Apply(img) } // Deskew using Deskew class
                    };

                    var extractedTexts = new Dictionary<string, string>();

                    // Apply transformations and extract text
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

                    // Call embedding generator asynchronously and await the result
                    var embeddings = await embeddingGenerator.GenerateEmbeddingsForModelsAsync(extractedTexts);

                    // Calculate and save similarity report
                    similarityCalculator.ComputeAndSaveReport(embeddings);
                    logger.LogInfo($"Successfully processed image: {fileName}{fileExtension}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing {inputFilePath}: {ex.Message}");
                    Console.WriteLine($"Error processing {inputFilePath}: {ex.Message}");
                }
            }

            // Generate final reports
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

    private static Image<Rgba32> ApplyShift(Image<Rgba32> image, Image<Rgba32> resizedImage, int shiftX, int shiftY)
    {
        // Get the dimensions of the image.
        int width = resizedImage.Width;
        int height = resizedImage.Height;

        // Ensure shift values stay within bounds.
        // For shiftX, we can restrict it to [-width+1, width-1], so shifting stays within bounds
        shiftX = Math.Clamp(shiftX, -width + 1, width - 1);
        shiftY = Math.Clamp(shiftY, -height + 1, height - 1);

        // Now apply the shift using the ShiftImage class
        return new ShiftImage().Apply(resizedImage, shiftX, shiftY);
    }


    private static Image<Rgba32> ApplySaturationAdjustment(Image<Rgba32> image, float saturation)
    {
        // Step 1: Extract pixel data manually by iterating over the image.
        int width = image.Width;
        int height = image.Height;

        byte[] imageData = new byte[width * height * 4]; // 4 bytes per pixel (RGBA)

        // Copy pixels from the image into the byte array manually
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = image[x, y]; // Get the pixel at position (x, y)
                int index = (y * width + x) * 4; // Calculate index for the pixel in the byte array

                imageData[index] = pixel.R;
                imageData[index + 1] = pixel.G;
                imageData[index + 2] = pixel.B;
                imageData[index + 3] = pixel.A;
            }
        }

        // Step 2: Apply the saturation adjustment using the SaturationAdjustment class
        var adjustedData = new SaturationAdjustment().Apply(imageData, width, height, 4, saturation);

        // Step 3: Convert the adjusted byte array back to an Image<Rgba32>
        return Image.LoadPixelData<Rgba32>(adjustedData, width, height);
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
