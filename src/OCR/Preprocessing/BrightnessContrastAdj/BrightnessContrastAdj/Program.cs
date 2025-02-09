using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Tesseract;

namespace BrightnessContrastOCR
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/BrighnessContrast_Test.jpeg";
                string outputPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/BrighnessContrastAdj_Test.jpeg";
                string logFilePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Logs/OCR_Log.txt";

                // Ensure the file exists
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Error: Image file not found!");
                    return;
                }

                // Get brightness and contrast values from the user
                Console.Write("Enter brightness adjustment (1.0 = no change, 1.2 = +20%): ");
                if (!float.TryParse(Console.ReadLine(), out float brightness))
                {
                    Console.WriteLine("Invalid brightness value. Exiting.");
                    return;
                }

                Console.Write("Enter contrast adjustment (1.0 = no change, 1.5 = +50%): ");
                if (!float.TryParse(Console.ReadLine(), out float contrast))
                {
                    Console.WriteLine("Invalid contrast value. Exiting.");
                    return;
                }

                // Load the image and apply preprocessing
                using (Image image = Image.Load(imagePath))
                {
                    Console.WriteLine("Converting to grayscale...");
                    image.Mutate(x => x.Grayscale());

                    Console.WriteLine("Adjusting brightness and contrast...");
                    image.Mutate(x => x.Brightness(brightness).Contrast(contrast));

                    image.Save(outputPath, new JpegEncoder());
                    Console.WriteLine($"Processed image saved at: {outputPath}");
                }

                // Perform OCR on both original and processed images
                Console.WriteLine("Performing OCR...");
                string originalText = PerformOCR(imagePath);
                string processedText = PerformOCR(outputPath);

                // Log results
                LogResults(logFilePath, imagePath, originalText, outputPath, processedText);
                Console.WriteLine("OCR process completed. Results logged.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        static string PerformOCR(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(@"/usr/local/share/tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OCR failed for {imagePath}: {ex.Message}");
                return "OCR Error";
            }
        }

        static void LogResults(string logFilePath, string originalPath, string originalText, string processedPath, string processedText)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("===== OCR Processing Log =====");
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine($"Original Image Path: {originalPath}");
                    writer.WriteLine("Original OCR Result:");
                    writer.WriteLine(originalText);
                    writer.WriteLine("------------------------------");
                    writer.WriteLine($"Processed Image Path: {processedPath}");
                    writer.WriteLine("Processed OCR Result:");
                    writer.WriteLine(processedText);
                    writer.WriteLine("================================\n");
                }
                Console.WriteLine($"OCR results logged to {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to write log file: " + ex.Message);
            }
        }
    }
}
