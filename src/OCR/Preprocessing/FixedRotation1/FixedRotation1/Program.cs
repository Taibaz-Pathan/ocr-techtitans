using System;
using System.Drawing; // For Bitmap
using AForge.Imaging.Filters; // For RotateBilinear filter

namespace FixedRotationTest
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/unmarshalling/Downloads/ocr-techtitans/Input/Test_2.jpg"; // Update path to your image
                string outputPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output"; // Output path for the rotated image

                // Ensure the file exists
                if (!System.IO.File.Exists(imagePath))
                {
                    Console.WriteLine("Image file not found!");
                    return;
                }

                // Load the image
                Bitmap originalImage = new Bitmap(imagePath);

                // Rotate the image by 90 degrees clockwise
                Console.WriteLine("Rotating image by 90 degrees...");
                RotateBilinear rotationFilter = new RotateBilinear(90);
                Bitmap rotatedImage = rotationFilter.Apply(originalImage);

                // Save the rotated image
                rotatedImage.Save(outputPath);
                Console.WriteLine($"Rotated image saved at: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}