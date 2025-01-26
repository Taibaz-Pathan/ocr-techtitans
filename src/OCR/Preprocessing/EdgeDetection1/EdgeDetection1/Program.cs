using System;
using Tesseract;
using OpenCvSharp;

namespace OCRProcessor
{
    class EdgeDetection
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Initializing Tesseract Engine...");
                var ocrEngine = new TesseractEngine("/Users/khushalsingh/Downloads/ocr-techtitans/src/OCR/OCRPreprocessor/Tesseract/ConsoleApp1/ConsoleApp1/tessdata", "eng", EngineMode.Default);

                using (ocrEngine)
                {
                    Console.WriteLine("Loading image...");
                    string inputImagePath = "/Users/khushalsingh/Downloads/ocr-techtitans/Input/Testing_edge_detection.jpg";

                    if (!System.IO.File.Exists(inputImagePath))
                    {
                        Console.WriteLine("Image file not found!");
                        return;
                    }

                    // Preprocess the image
                    string preprocessedImagePath = PreprocessImage(inputImagePath);

                    // Load preprocessed image into Tesseract
                    using (var img = Pix.LoadFromFile(preprocessedImagePath))
                    {
                        Console.WriteLine("Processing image...");
                        var result = ocrEngine.Process(img);

                        Console.WriteLine("Extracted Text:");
                        Console.WriteLine(result.GetText());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString()); // Full stack trace
            }
        }

        static string PreprocessImage(string inputPath)
        {
            Console.WriteLine("Preprocessing image for edge detection...");

            // Load the image in grayscale
            Mat image = Cv2.ImRead(inputPath, ImreadModes.Grayscale);

            // Apply Gaussian blur to reduce noise
            Mat blurred = new Mat();
            Cv2.GaussianBlur(image, blurred, new Size(5, 5), 0);

            // Apply Canny edge detection
            Mat edges = new Mat();
            Cv2.Canny(blurred, edges, 50, 150);

            // Save the preprocessed image
            string outputDirectory = "/Users/khushalsingh/Downloads/ocr-techtitans/Output";
            if (!System.IO.Directory.Exists(outputDirectory))
            {
                System.IO.Directory.CreateDirectory(outputDirectory);
            }

            string outputPath = System.IO.Path.Combine(outputDirectory, "EdgeDetectedImage.jpg");
            Cv2.ImWrite(outputPath, edges);

            Console.WriteLine($"Preprocessed image saved at: {outputPath}");
            return outputPath;
        }
    }
}
