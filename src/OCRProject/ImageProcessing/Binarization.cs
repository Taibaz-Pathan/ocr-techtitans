using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OCRProject.ImageProcessing
{
    public class Binarization
    {
        public Bitmap ApplyBinarization(Bitmap image, float threshold)
        {
            Bitmap binarizedImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int gray = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);
                    Color newColor = gray < (threshold * 255) ? Color.Black : Color.White;
                    binarizedImage.SetPixel(x, y, newColor);
                }
            }

            return binarizedImage;
        }

        public void SaveBinarizedImage(Bitmap image, string outputPath)
        {
            try
            {
                image.Save(outputPath, ImageFormat.Png);
                Console.WriteLine($"Binarized image saved at: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving image: " + ex.Message);
            }
        }
    }
}