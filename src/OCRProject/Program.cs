using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using OCRProject.ImageProcessing;
using OCRProject.Utils;
using OCRProject.TesseractProcessor;
using OCRProject.ModelComparison;
using ModelComparison;

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

            if (createdFilePath == null)
            {
                logger.LogError("Failed to create text file.");
                Console.WriteLine("Failed to create text file.");
                return;
            }

            logger.LogInfo($"Text file created successfully: {createdFilePath}");

            var fileWriter = new FileWriter();
            var timeTracker = new ProcessingTimeTracker(comparisonResults);
            var similarityCalculator = new CosineSimilarityCalculator(comparisonResults);

            var imageFiles = Directory.GetFiles(inputFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (imageFiles.Length == 0)
            {
                logger.LogError("No images found in the input folder.");
                Console.WriteLine("No images found in the input folder.");
                return;
            }

            logger.LogInfo($"Processing {imageFiles.Length} images...");

            foreach (var inputFilePath in imageFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    string fileExtension = Path.GetExtension(inputFilePath).ToLower();

                    using (var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                    using (var inputImage = Image.FromStream(fileStream, false, false))
                    {
                        using (var resizedImage = ResizeImage(inputImage, 2000, 2000))
                        {
                            logger.LogInfo($"Processing image: {fileName}{fileExtension}");

                            var transformations = new Dictionary<string, Func<Bitmap, Bitmap>>
                            {
                                { "Grayscale", img => new ConvertToGrayscale().Apply(img) },
                                { "GlobalThreshold", img => new GlobalThresholding(128).ApplyThreshold(img) },
                                { "Shifted", img => new ShiftImage().Apply(img, 5, 5) },
                                { "SaturationAdjusted", img => new SaturationAdjustment().Apply(img, 1.2f) },
                                { "Deskewed", img => new Deskew().Apply(img) }
                            };

                            var extractedTexts = new Dictionary<string, string>();

                            foreach (var (modelName, transform) in transformations)
                            {
                                timeTracker.StartTimer();
                                using (var processedImage = transform(new Bitmap(resizedImage)))
                                {
                                    string outputPath = Path.Combine(outputFolderImage, fileName + "_" + modelName + fileExtension);
                                    processedImage.Save(outputPath, GetImageFormat(fileExtension));
                                    logger.LogInfo($"Saved processed image: {outputPath}");

                                    string extractedText = TesseractProcessor.ExtractTextFromImage(processedImage, createdFilePath, fileWriter);
                                    extractedTexts[modelName] = extractedText;
                                    timeTracker.StopAndRecord(fileName, modelName);
                                }
                            }

                            var embeddings = embeddingGenerator.GenerateEmbeddingsForModels(extractedTexts);
                            similarityCalculator.ComputeAndSaveReport(embeddings);
                            logger.LogInfo($"Successfully processed image: {fileName}{fileExtension}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing {inputFilePath}: {ex.Message}");
                    Console.WriteLine($"Error processing {inputFilePath}: {ex.Message}");
                }
            }

            timeTracker.GenerateExcelReport();
            logger.LogInfo("Processing completed successfully.");
            Console.WriteLine("Processing completed. Press any key to exit.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Critical error in process: {ex.Message}");
            Console.WriteLine($"Error in Process: {ex.Message}");
        }
        finally
        {
            Console.ReadKey();
        }
    }

    private static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
    {
        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Min(ratioX, ratioY);
        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        var newImage = new Bitmap(newWidth, newHeight);
        using (var graphics = Graphics.FromImage(newImage))
        {
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        return newImage;
    }

    private static ImageFormat GetImageFormat(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".bmp" => ImageFormat.Bmp,
            ".gif" => ImageFormat.Gif,
            ".tiff" => ImageFormat.Tiff,
            _ => ImageFormat.Png,
        };
    }
}
