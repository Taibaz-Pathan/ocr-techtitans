using System;
using System.IO;
using OpenCvSharp;
using Tesseract;

namespace OCRModule
{
    class Program
    {
        static void Main()
        {
            try
            {
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/sample.jpg";
                string extractedText = OCRProcessor.ExtractTextFromImage(imagePath);

                Console.WriteLine("\nExtracted Text:\n" + extractedText);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString());
            }
        }
    }

    static class OCRProcessor
    {
        /// <summary>
        /// Extracts text from an image using Tesseract OCR.
        /// </summary>
        public static string ExtractTextFromImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("Image file not found!");
                return string.Empty;
            }

            // Load and preprocess the image
            Mat image = Cv2.ImRead(imagePath, ImreadModes.Grayscale);
            Cv2.Threshold(image, image, 128, 255, ThresholdTypes.Binary); // Binarization

            // Save preprocessed image temporarily
            string tempImagePath = Path.Combine(Path.GetTempPath(), "processed_image.jpg");
            Cv2.ImWrite(tempImagePath, image);

            // Path to Tesseract data folder (Update if necessary)
            string tessDataPath = @"/usr/local/share/tessdata"; // Change for Windows (C:\Program Files\Tesseract-OCR\tessdata)

            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(tempImagePath))
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
                Console.WriteLine("Error in OCR processing: " + ex.Message);
                return string.Empty;
            }
            finally
            {
                // Clean up temporary files
                if (File.Exists(tempImagePath))
                {
                    File.Delete(tempImagePath);
                }
            }
        }
    }
}
