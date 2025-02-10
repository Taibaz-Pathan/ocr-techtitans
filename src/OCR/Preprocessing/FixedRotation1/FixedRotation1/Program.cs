using System;
using System.IO;
using OpenCvSharp;  // For OpenCV (Auto Alignment)
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Tesseract; // For OCR

namespace FixedRotationTest
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/AlignmentTest.jpeg";
                string fixedRotationPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/auto_align90.jpg";
                string autoAlignPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/auto_align.jpg";
                string logFilePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Logs/ocr_log_FixedRotation.txt";

                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Image file not found!");
                    return;
                }

                Console.WriteLine("\nChoose Rotation Mode:");
                Console.WriteLine("1. Fixed 90-degree Rotation");
                Console.WriteLine("2. Auto Alignment (Correct Skew)");
                Console.Write("Enter option (1/2): ");
                string? choice = Console.ReadLine();
                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                    return;
                }

                string processedImagePath = imagePath;
                if (choice == "1")
                {
                    PreprocessingManager.RotateImageFixed(imagePath, fixedRotationPath, 90);
                    Console.WriteLine($"Rotated image saved at: {fixedRotationPath}");
                    processedImagePath = fixedRotationPath;
                }
                else if (choice == "2")
                {
                    PreprocessingManager.AutoAlignImage(imagePath, autoAlignPath);
                    Console.WriteLine($"Auto-aligned image saved at: {autoAlignPath}");
                    processedImagePath = autoAlignPath;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please enter 1 or 2.");
                    return;
                }

                // Perform OCR on original and processed image
                string ocrResultOriginal = OCRProcessor.PerformOCR(imagePath);
                string ocrResultProcessed = OCRProcessor.PerformOCR(processedImagePath);

                // Log OCR results
                File.WriteAllText(logFilePath, "OCR Results:\n");
                File.AppendAllText(logFilePath, $"Original Image OCR:\n{ocrResultOriginal}\n\n");
                File.AppendAllText(logFilePath, $"Processed Image OCR:\n{ocrResultProcessed}\n");

                Console.WriteLine("OCR results logged successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString());
            }
        }
    }

    static class PreprocessingManager
    {
        public static void RotateImageFixed(string inputPath, string outputPath, float angle)
        {
            using (Image image = Image.Load(inputPath))
            {
                image.Mutate(x => x.Rotate(angle));
                image.Save(outputPath, new JpegEncoder());
            }
        }

        public static void AutoAlignImage(string inputPath, string outputPath)
        {
            Mat image = Cv2.ImRead(inputPath, ImreadModes.Grayscale);
            Mat edges = new Mat();
            Cv2.Canny(image, edges, 50, 150);

            LineSegmentPolar[] lines = Cv2.HoughLines(edges, 1, Math.PI / 180, 200);
            double detectedAngle = 0;
            
            if (lines.Length > 0)
            {
                detectedAngle = lines[0].Theta * (180 / Math.PI); // Convert radians to degrees
            }

            RotateImageFixed(inputPath, outputPath, (float)-detectedAngle);
        }
    }

    static class OCRProcessor
    {
        public static string PerformOCR(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error performing OCR: {ex.Message}";
            }
        }
    }
}
