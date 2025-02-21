using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;

class Binarization
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
            string outputPath = $"/Users/khushalsingh/Downloads/ocr-techtitans/Output/binarized_{timestamp}.jpg";

            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                Console.WriteLine("Applying binarization...");
                image.Mutate(ctx => ctx.BinaryThreshold(0.5f)); // Convert to black & white (50% threshold)

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