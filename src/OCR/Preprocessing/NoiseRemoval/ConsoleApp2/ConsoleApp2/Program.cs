using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

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
                Console.WriteLine("Image file not found!");
                return;
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/noise_removed_{timestamp}.jpg";

            using (Image image = Image.Load(imagePath))
            {
                Console.WriteLine("Applying noise removal...");
                image.Mutate(x => x.GaussianBlur(1.5f));  // Using Gaussian Blur as a noise reduction filter

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