using System;
using Tesseract;

class Program
{
    static void Main()
    {
        string imagePath = "test-image.png"; // Ensure this file is present in the project folder
        string tessDataPath = @"./tessdata"; // Download and place the tessdata folder

        var ocrEngine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
        using (var img = Pix.LoadFromFile(imagePath))
        {
            using (var page = ocrEngine.Process(img))
            {
                Console.WriteLine("Extracted Text:");
                Console.WriteLine(page.GetText());
            }
        }
    }
}