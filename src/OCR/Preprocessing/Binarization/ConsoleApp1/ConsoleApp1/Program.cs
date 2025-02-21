using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BinarizationOCR
{
    class Binarization
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/sample.jpg";

                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Error: Image file not found!");
                    return;
                }

                Console.Write("Enter binarization threshold (0 to 1, e.g., 0.5): ");
                if (!float.TryParse(Console.ReadLine(), out float threshold) || threshold < 0 || threshold > 1)
                {
                    Console.WriteLine("Invalid input. Using default threshold: 0.5");
                    threshold = 0.5f;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/binarized_{timestamp}.jpg";
                string logFilePath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Logs/Binarization_Log_{timestamp}.txt";

                using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
                {
                    Console.WriteLine("Applying binarization...");
                    image.Mutate(ctx => ctx.BinaryThreshold(threshold));
                    image.Save(outputPath, new JpegEncoder());
                }

                if (File.Exists(outputPath))
                {
                    Console.WriteLine($"Binarized image saved at: {outputPath}");
                    LogResults(logFilePath, imagePath, outputPath, threshold);
                }
                else
                {
                    Console.WriteLine("Error: Processed image not saved.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        static void LogResults(string logFilePath, string originalPath, string processedPath, float threshold)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, false)) // Overwrites each time
                {
                    writer.WriteLine("===== Binarization Processing Log =====");
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine($"Original Image Path: {originalPath}");
                    writer.WriteLine($"Processed Image Path: {processedPath}");
                    writer.WriteLine($"Binarization Threshold: {threshold}");
                    writer.WriteLine("=======================================");
                }
                Console.WriteLine($"Binarization log saved at: {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write log file: " + ex.Message);
            }
        }
    }
}
