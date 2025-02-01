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

                // Get brightness and contrast values from the user
                Console.Write("Enter brightness adjustment (1.0 = no change, 1.2 = +20%): ");
                float brightness = float.Parse(Console.ReadLine());

                Console.Write("Enter contrast adjustment (1.0 = no change, 1.5 = +50%): ");
                float contrast = float.Parse(Console.ReadLine());

                // Load the image
                using (Image image = Image.Load(imagePath))
                {
                    // Convert to grayscale
                    Console.WriteLine("Converting to grayscale...");
                    image.Mutate(x => x.Grayscale());

                    // Adjust brightness and contrast based on user input
                    Console.WriteLine("Adjusting brightness and contrast...");
                    image.Mutate(x => x.Brightness(brightness).Contrast(contrast));

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
