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

                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("❌ Error: Image file not found! Please check the file path.");
                    return;
                }

                Console.Write("Enter new width: ");
                if (!int.TryParse(Console.ReadLine(), out int newWidth) || newWidth <= 0)
                {
                    Console.WriteLine("⚠️ Invalid width value. Please enter a positive integer.");
                    return;
                }

                Console.Write("Enter new height: ");
                if (!int.TryParse(Console.ReadLine(), out int newHeight) || newHeight <= 0)
                {
                    Console.WriteLine("⚠️ Invalid height value. Please enter a positive integer.");
                    return;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputDir = "/Users/khushalsingh/Downloads/ocr-techtitans/Output/";
                string logDir = "/Users/khushalsingh/Downloads/ocr-techtitans/Logs/";

                Directory.CreateDirectory(outputDir);
                Directory.CreateDirectory(logDir);

                string outputPath = $"{outputDir}resized_{timestamp}.jpg";
                string logFilePath = $"{logDir}Resize_Log_{timestamp}.txt";

                try
                {
                    using (Image image = Image.Load(imagePath))
                    {
                        Console.WriteLine("🔄 Resizing image...");
                        image.Mutate(x => x.Resize(newWidth, newHeight));

                        image.Save(outputPath, new JpegEncoder());
                        Console.WriteLine($"✅ Resized image saved at: {outputPath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error loading or processing image: {ex.Message}");
                    return;
                }

                LogResults(logFilePath, imagePath, outputPath, newWidth, newHeight);
                Console.WriteLine("✅ Image resizing completed. Results logged.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An unexpected error occurred: {ex.Message}");
            }
        }

        static void LogResults(string logFilePath, string originalPath, string processedPath, int width, int height)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, false))
                {
                    writer.WriteLine("===== Image Resizing Log =====");
                    writer.WriteLine($"🕒 Timestamp: {DateTime.Now}");
                    writer.WriteLine($"📂 Original Image Path: {originalPath}");
                    writer.WriteLine($"💾 Processed Image Path: {processedPath}");
                    writer.WriteLine($"📏 New Dimensions: {width}x{height}");
                    writer.WriteLine("================================");
                }
                Console.WriteLine($"📄 Image resizing log saved to {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to write log file: {ex.Message}");
            }
        }
    }
}
