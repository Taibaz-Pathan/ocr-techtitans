using System;
using SixLabors.ImageSharp;  // For ImageSharp
using SixLabors.ImageSharp.Processing;  // For image processing (rotate)
using SixLabors.ImageSharp.Formats.Jpeg;  // For saving as JPEG

namespace FixedRotationTest
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/Test_2.jpg"; // Update path to your image
                string outputPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/rotated_image.jpg"; // Output path for the rotated image

                // Ensure the file exists
                if (!System.IO.File.Exists(imagePath))
                {
                    Console.WriteLine("Image file not found!");
                    return;
                }

                // Load the image using ImageSharp
                using (Image image = Image.Load(imagePath))
                {
                    // Rotate the image by 90 degrees clockwise
                    Console.WriteLine("Rotating image by 90 degrees...");
                    image.Mutate(x => x.Rotate(90));

                    // Save the rotated image
                    image.Save(outputPath, new JpegEncoder());
                    Console.WriteLine($"Rotated image saved at: {outputPath}");
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