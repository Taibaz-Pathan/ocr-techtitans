﻿using System;
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
                var ocrEngine = new TesseractEngine("/Users/taibazpathan/Desktop/Git/test/ConsoleAppTesst", "eng", EngineMode.Default);

                using (ocrEngine)
                {
                    Console.WriteLine("Loading image...");
                    if (!System.IO.File.Exists("/Users/taibazpathan/Desktop/Git/test/ConsoleAppTesst/test_1.jpg"))
                    {
                        Console.WriteLine("Image file not found!");
                        return;
                    }

                    using (var img = Pix.LoadFromFile("/Users/taibazpathan/Desktop/Git/test/ConsoleAppTesst/test_1.jpg"))
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
    }
}