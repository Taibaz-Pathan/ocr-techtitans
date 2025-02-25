using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageProcessing
{
    class NoiseRemoval
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

                Console.Write("Enter blur intensity (recommended: 1.5): ");
                if (!float.TryParse(Console.ReadLine(), out float blurIntensity) || blurIntensity <= 0)
                {
                    Console.WriteLine("Invalid input! Using default blur intensity: 1.5");
                    blurIntensity = 1.5f;
                }

                string outputDirectory = "/Users/khushalsingh/Downloads/ocr-techtitans/Output/";
                string logDirectory = "/Users/khushalsingh/Downloads/ocr-techtitans/Logs/";
                Directory.CreateDirectory(outputDirectory);
                Directory.CreateDirectory(logDirectory);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"{outputDirectory}noise_removed_{timestamp}.jpg";
                string logFilePath = $"{logDirectory}NoiseRemoval_Log_{timestamp}.txt";

                try
                {
                    using (Image image = Image.Load(imagePath))
                    {
                        Console.WriteLine($"Applying noise removal with intensity {blurIntensity}...");
                        image.Mutate(x => x.GaussianBlur(blurIntensity));
                        image.Save(outputPath, new JpegEncoder());
                    }

                    if (File.Exists(outputPath))
                    {
                        Console.WriteLine($"Noise-removed image saved at: {outputPath}");
                        LogResults(logFilePath, imagePath, outputPath, blurIntensity);
                    }
                    else
                    {
                        Console.WriteLine("Error: Processed image not saved!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to process image: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
