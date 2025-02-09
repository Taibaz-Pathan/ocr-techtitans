using System;
using System.IO;
using Tesseract;

namespace OCRProcessor
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Initializing Tesseract Engine...");
                var ocrEngine = new TesseractEngine(
                    "/Users/khushalsingh/Downloads/ocr-techtitans/src/OCR/OCRPreprocessor/Tesseract/ConsoleApp1/ConsoleApp1/tessdata",
                    "eng",
                    EngineMode.Default
                );

                using (ocrEngine)
                {
                    string inputImagePath = "/Users/khushalsingh/Downloads/ocr-techtitans/Input/TextExtraction_Test.jpg";
                    string outputTextPath = "/Users/khushalsingh/Downloads/ocr-techtitans/Output/TextExtraction_Test.txt";

                    Console.WriteLine("Loading image...");
                    if (!File.Exists(inputImagePath))
                    {
                        Console.WriteLine("Image file not found!");
                        return;
                    }

                    using (var img = Pix.LoadFromFile(inputImagePath))
                    {
                        Console.WriteLine("Processing image...");
                        var result = ocrEngine.Process(img);

                        string extractedText = result.GetText().Trim();

                        // Save extracted text to output file
                        File.WriteAllText(outputTextPath, extractedText);

                        Console.WriteLine("\nExtracted Text:");
                        Console.WriteLine(extractedText);
                        Console.WriteLine($"\nExtracted text has been saved at: {outputTextPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString()); // Full stack trace
            }
        }
    }
}