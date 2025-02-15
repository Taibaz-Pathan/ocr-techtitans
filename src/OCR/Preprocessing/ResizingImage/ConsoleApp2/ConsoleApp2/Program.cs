using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

class ImageResizer
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

            int newWidth = 800;  // Set your desired width
            int newHeight = 600; // Set your desired height

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/resized_{timestamp}.jpg";

            using (Image image = Image.Load(imagePath))
            {
                Console.WriteLine("Resizing image...");
                image.Mutate(x => x.Resize(newWidth, newHeight));

                image.Save(outputPath, new JpegEncoder());
                Console.WriteLine($"Resized image saved at: {outputPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}