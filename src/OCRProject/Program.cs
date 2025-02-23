using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Concurrent;
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
        try
        {
            Console.WriteLine("Starting Image Processing...");

            // Load configuration
            var config = new ConfigLoader();
            string inputFolder = config.InputFolder;
            string outputFolderImage = config.OutputImageFolder;
            string outputFolderText = config.ExtractedTextFolder;
            string comparisonResults = config.ComparisionFolder;

            // Create output text file
            var fileCreator = new CreateFiles();
            string fileNameExtracted = $"ProcessedFile_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string createdFilePath = fileCreator.CreateTextFile(outputFolderText, fileNameExtracted);

            Console.WriteLine(createdFilePath != null
                ? $"Text file created successfully: {createdFilePath}"
                : "Failed to create text file.");

            var fileWriter = new FileWriter();

            // Parallel processing of images
            var imageFiles = Directory.GetFiles(inputFolder, "*.png");
            var timeTracker = new ProcessingTimeTracker(comparisonResults);

            Parallel.ForEach(imageFiles, inputFilePath =>
            {
                
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    using var inputImage = new Bitmap(inputFilePath);
                    ImageDisplayer.ShowImage(inputImage, "Original Image");

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

                        TesseractProcessor.ExtractTextFromImage(processedImage, createdFilePath, fileWriter);
                        timeTracker.StopAndRecord(fileName, suffix);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {inputFilePath}: {ex.Message}");
                }
            });

            timeTracker.GenerateExcelReport();
            Console.WriteLine("Processing completed. Press any key to exit.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Process: {ex.Message}");
        }
        finally
        {
            Console.ReadKey();
        }
    }
}
