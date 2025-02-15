using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageProcessing
{
    class ImageResizer
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/sample.jpg";

                // Ensure the image file exists
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Error: Image file not found!");
                    return;
                }

                // Define new dimensions
                Console.Write("Enter new width: ");
                if (!int.TryParse(Console.ReadLine(), out int newWidth) || newWidth <= 0)
                {
                    Console.WriteLine("Invalid width value. Exiting.");
                    return;
                }

                Console.Write("Enter new height: ");
                if (!int.TryParse(Console.ReadLine(), out int newHeight) || newHeight <= 0)
                {
                    Console.WriteLine("Invalid height value. Exiting.");
                    return;
                }

                // Generate unique output file and log file names
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/resized_{timestamp}.jpg";
                string logFilePath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Logs/Resize_Log_{timestamp}.txt";

                // Load the image and apply resizing
                using (Image image = Image.Load(imagePath))
                {
                    Console.WriteLine("Resizing image...");
                    image.Mutate(x => x.Resize(newWidth, newHeight));

                    image.Save(outputPath, new JpegEncoder());
                    Console.WriteLine($"Resized image saved at: {outputPath}");
                }

                // Log the results
                LogResults(logFilePath, imagePath, outputPath, newWidth, newHeight);
                Console.WriteLine("Image resizing completed. Results logged.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        static void LogResults(string logFilePath, string originalPath, string processedPath, int width, int height)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, false))
                {
                    writer.WriteLine("===== Image Resizing Log =====");
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine($"Original Image Path: {originalPath}");
                    writer.WriteLine($"Processed Image Path: {processedPath}");
                    writer.WriteLine($"New Dimensions: {width}x{height}");
                    writer.WriteLine("================================");
                }
                Console.WriteLine($"Image resizing log saved to {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write log file: " + ex.Message);
            }
        }
    }
}
