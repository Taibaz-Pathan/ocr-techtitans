using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using OCRProject.ImageProcessing;
using OCRProject.Utils;
using OCRProject.TesseractProcessor;
using OCRProject.ModelComparison;
using Utils;

class Program
{
    static void Main(string[] args)
    {
        var logger = new Logger(); // Initialize logger

        try
        {
            Console.WriteLine("Starting Image Processing...");
            logger.LogInfo("Starting Image Processing...");

            // Load configuration
            var config = new ConfigLoader();
            string inputFolder = config.InputFolder;
            string outputFolderImage = config.OutputImageFolder;
            string outputFolderText = config.ExtractedTextFolder;
            string comparisonResults = config.ComparisionFolder;

          
            logger.LogInfo("Configuration loaded successfully.");

            // Create output text file
            var fileCreator = new CreateFiles();
            string fileNameExtracted = $"ProcessedFile_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string createdFilePath = fileCreator.CreateTextFile(outputFolderText, fileNameExtracted);

            if (createdFilePath != null)
            {
                logger.LogInfo($"Text file created successfully: {createdFilePath}");
                Console.WriteLine($"Text file created successfully: {createdFilePath}");
            }
            else
            {
                logger.LogError("Failed to create text file.");
                Console.WriteLine("Failed to create text file.");
                return; // Stop execution if text file cannot be created
            }

            var fileWriter = new FileWriter();
            var imageFiles = Directory.GetFiles(inputFolder, "*.png");
            var timeTracker = new ProcessingTimeTracker(comparisonResults);

            if (imageFiles.Length == 0)
            {
                logger.LogError("No images found in the input folder.");
                Console.WriteLine("No images found in the input folder.");
                return;
            }

            logger.LogInfo($"Processing {imageFiles.Length} images...");

            // Parallel processing of images
            Parallel.ForEach(imageFiles, inputFilePath =>
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    using var inputImage = new Bitmap(inputFilePath);
                    ImageDisplayer.ShowImage(inputImage, "Original Image");

                    logger.LogInfo($"Processing image: {fileName}.png");

                    var transformations = new (string, Func<Bitmap, Bitmap>)[]
                    {
                        ("_grayscale", img => new ConvertToGrayscale().Apply(img)),
                        ("_AdaptiveThreshold", img => new AdaptiveThreshold().ApplyThreshold(img)),
                        ("_GlobalThreshold", img => new GlobalThresholding(128).ApplyThreshold(img)),
                        ("_Shifted", img => new ShiftImage().Apply(img, 5, 5)),
                        ("_SaturationAdjusted", img => new SaturationAdjustment().Apply(img, 1.2f)),
                        ("_Deskewed", img => new Deskew().Apply(img))
                    };

                    foreach (var (suffix, transform) in transformations)
                    {
                        timeTracker.StartTimer();
                        using var processedImage = transform(inputImage);
                        string outputPath = Path.Combine(outputFolderImage, fileName + suffix + ".png");
                        processedImage.Save(outputPath, ImageFormat.Png);

                        logger.LogInfo($"Saved processed image: {outputPath}");

                        TesseractProcessor.ExtractTextFromImage(processedImage, createdFilePath, fileWriter);
                        timeTracker.StopAndRecord(fileName, suffix);
                    }

                    logger.LogInfo($"Successfully processed image: {fileName}.png");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing {inputFilePath}: {ex.Message}");
                    Console.WriteLine($"Error processing {inputFilePath}: {ex.Message}");
                }
            });

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
}
