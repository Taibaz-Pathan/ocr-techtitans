using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BrightnessContrastAdjustment
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/BrighnessContrast_Test.jpeg"; // Path to the input image
                string outputPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/BrighnessContrastAdj_Test.jpeg"; // Output path

                // Ensure the file exists
                if (!System.IO.File.Exists(imagePath))
                {
                    Console.WriteLine("Image file not found!");
                    return;
                }

                // Load the image
                using (Image image = Image.Load(imagePath))
                {
                    // Convert to grayscale
                    Console.WriteLine("Converting to grayscale...");
                    image.Mutate(x => x.Grayscale());

                    // Adjust brightness and contrast
                    Console.WriteLine("Adjusting brightness and contrast...");
                    image.Mutate(x => x.Brightness(1.2f).Contrast(1.5f)); // Brightness (1.2 = +20%), Contrast (1.5 = +50%)

                    // Save the adjusted image
                    image.Save(outputPath, new JpegEncoder());
                    Console.WriteLine($"Adjusted image saved at: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}