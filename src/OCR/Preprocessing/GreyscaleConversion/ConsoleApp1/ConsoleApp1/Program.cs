using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace GrayscaleOCR
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/ColourAdjTest.jpg";

                // Generate a new output file name based on the current timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/greyscale_{timestamp}.jpg";
                string logFilePath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Logs/Greyscale_Log_{timestamp}.txt";

                // Ensure the image file exists
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Error: Image file not found!");
                    return;
                }

                // Load the image and apply preprocessing
                using (Image image = Image.Load(imagePath))
                {
                    Console.WriteLine("Converting to grayscale...");
                    image.Mutate(x => x.Grayscale());

                    image.Save(outputPath, new JpegEncoder());

                    // Check if the image is saved successfully
                    if (!File.Exists(outputPath))
                    {
                        Console.WriteLine("Error: Processed image not saved.");
                        return;
                    }

                    Console.WriteLine($"Processed image saved at: {outputPath}");
                }

                // Log results
                LogResults(logFilePath, imagePath, outputPath);
                Console.WriteLine("Grayscale conversion completed. Results logged.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        static void LogResults(string logFilePath, string originalPath, string processedPath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, false)) // 'false' ensures overwriting the file each time
                {
                    writer.WriteLine("===== Grayscale Processing Log =====");
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine($"Original Image Path: {originalPath}");
                    writer.WriteLine($"Processed Image Path: {processedPath}");
                    writer.WriteLine("================================");
                }
                Console.WriteLine($"Grayscale conversion log saved to {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write log file: " + ex.Message);
            }
        }
    }
}
