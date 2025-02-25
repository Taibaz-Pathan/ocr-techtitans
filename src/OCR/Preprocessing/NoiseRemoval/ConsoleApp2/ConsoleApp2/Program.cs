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

                // Ask user for noise removal intensity
                Console.Write("Enter blur intensity (recommended: 1.5): ");
                if (!float.TryParse(Console.ReadLine(), out float blurIntensity) || blurIntensity <= 0)
                {
                    Console.WriteLine("Invalid input! Using default blur intensity: 1.5");
                    blurIntensity = 1.5f;
                }

                // Ensure output directory exists
                string outputDirectory = "/Users/khushalsingh/Downloads/ocr-techtitans/Output/";
                Directory.CreateDirectory(outputDirectory);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"{outputDirectory}noise_removed_{timestamp}.jpg";

                using (Image image = Image.Load(imagePath))
                {
                    Console.WriteLine($"Applying noise removal with intensity {blurIntensity}...");
                    image.Mutate(x => x.GaussianBlur(blurIntensity));
                    image.Save(outputPath, new JpegEncoder());
                    Console.WriteLine($"Noise-removed image saved at: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}