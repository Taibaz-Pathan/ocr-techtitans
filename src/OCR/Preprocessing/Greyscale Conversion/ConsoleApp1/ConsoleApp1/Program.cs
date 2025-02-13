using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

class GrayscaleConverter
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
            string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/grayscale_{timestamp}.jpg";

            using (Image image = Image.Load(imagePath))
            {
                Console.WriteLine("Converting to grayscale...");
                image.Mutate(x => x.Grayscale());

                image.Save(outputPath, new JpegEncoder());
                Console.WriteLine($"Grayscale image saved at: {outputPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}