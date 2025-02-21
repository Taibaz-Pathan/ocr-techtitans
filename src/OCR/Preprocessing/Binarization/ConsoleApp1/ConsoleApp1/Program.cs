using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BinarizationOCR
{
    class Binarization
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/sample.jpg";

                // Ensure the image file exists
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Error: Image file not found!");
                    return;
                }

                Console.Write("Enter binarization threshold (0 to 1, e.g., 0.5): ");
                if (!float.TryParse(Console.ReadLine(), out float threshold) || threshold < 0 || threshold > 1)
                {
                    Console.WriteLine("Invalid input. Using default threshold: 0.5");
                    threshold = 0.5f;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/binarized_{timestamp}.jpg";

                // Load and process image
                using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
                {
                    Console.WriteLine("Applying binarization...");
                    image.Mutate(ctx => ctx.BinaryThreshold(threshold));

                    image.Save(outputPath, new JpegEncoder());

                    Console.WriteLine($"Binarized image saved at: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}