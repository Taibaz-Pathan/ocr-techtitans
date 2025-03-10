using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

            // Get all image files in the input folder (supporting multiple formats)
            var imageFiles = Directory.GetFiles(inputFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var timeTracker = new ProcessingTimeTracker(comparisonResults);

            if (imageFiles.Length == 0)
            {
                logger.LogError("No images found in the input folder.");
                Console.WriteLine("No images found in the input folder.");
                return;
            }

            logger.LogInfo($"Processing {imageFiles.Length} images...");

            // Process images sequentially to avoid memory issues with large images
            foreach (var inputFilePath in imageFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    string fileExtension = Path.GetExtension(inputFilePath).ToLower();

                    // Load the image in a memory-efficient way
                    using (var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                    using (var inputImage = Image.FromStream(fileStream, false, false))
                    {
                        // Resize large images to a manageable size (e.g., max width/height of 2000 pixels)
                        using (var resizedImage = ResizeImage(inputImage, 2000, 2000))
                        {
                            ImageDisplayer.ShowImage(resizedImage, "Original Image");

                            logger.LogInfo($"Processing image: {fileName}{fileExtension}");

                            var transformations = new (string, Func<Bitmap, Bitmap>)[]
                            {
                                ("_grayscale", img => new ConvertToGrayscale().Apply(img)),
                                ("_AdaptiveThreshold", img => new AdaptiveThreshold().ApplyThreshold(img)),
                                ("_GlobalThreshold", img => new GlobalThresholding(128).ApplyThreshold(img)),
                                ("_Shifted", img => new ShiftImage().Apply(img, 5, 5)),
                                ("_SaturationAdjusted", img => new SaturationAdjustment().Apply(img, 1.2f)),
                                ("_Deskewed", img => new Deskew().Apply(img)),
                                ("_Contrast", img => new Contrast().AdjustContrast(img, 1.2f)) // Adjust brightness & contrast
                                ("_GaussianBlur", img => new GaussianBlur().Apply(img, 2.0f)) // Apply Gaussian blur
                            };

                            foreach (var (suffix, transform) in transformations)
                            {
                                timeTracker.StartTimer();
                                using (var processedImage = transform(new Bitmap(resizedImage)))
                                {
                                    string outputPath = Path.Combine(outputFolderImage, fileName + suffix + fileExtension);

                                    // Save the processed image in the same format as the input
                                    processedImage.Save(outputPath, GetImageFormat(fileExtension));

                                    logger.LogInfo($"Saved processed image: {outputPath}");

                                    TesseractProcessor.ExtractTextFromImage(processedImage, createdFilePath, fileWriter);
                                    timeTracker.StopAndRecord(fileName, suffix);
                                }
                            }

                            logger.LogInfo($"Successfully processed image: {fileName}{fileExtension}");
                        }
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    logger.LogError($"Out of memory while processing {inputFilePath}: {ex.Message}");
                    Console.WriteLine($"Out of memory while processing {inputFilePath}: {ex.Message}");
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

    // Helper method to resize large images
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
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        return newImage;
    }

    // Helper method to get the correct ImageFormat based on file extension
    private static ImageFormat GetImageFormat(string extension)
    {
        switch (extension.ToLower())
        {
            case ".jpg":
            case ".jpeg":
                return ImageFormat.Jpeg;
            case ".bmp":
                return ImageFormat.Bmp;
            case ".gif":
                return ImageFormat.Gif;
            case ".tiff":
                return ImageFormat.Tiff;
            case ".png":
            default:
                return ImageFormat.Png;
        }
    }
}