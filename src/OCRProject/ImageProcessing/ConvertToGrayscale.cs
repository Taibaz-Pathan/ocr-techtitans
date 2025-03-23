using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace OCRProject.ImageProcessing
{
    public class ConvertToGrayscale
    {
        public Image<L8> Apply(Image<Rgba32> original)
        {
            // Clone the original image to avoid modifying it directly
            Image<L8> grayscaleImage = original.CloneAs<L8>();

            // Apply grayscale processing (looping manually, similar to original code)
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    Rgba32 pixelColor = original[x, y];

                    // Calculate grayscale value using luminance formula
                    byte gray = (byte)(0.3 * pixelColor.R + 0.59 * pixelColor.G + 0.11 * pixelColor.B);

                    // Set the grayscale pixel value
                    grayscaleImage[x, y] = new L8(gray);
                }
            }

            return grayscaleImage; // Return the processed grayscale image
        }
    }
}
