using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace OCRProject.ImageProcessing
{
    public class ConvertToGrayscale
    {
        /// <summary>
        /// Converts an RGB image to grayscale using a luminance-based formula.
        /// </summary>
        /// <param name="original">The input RGB image.</param>
        /// <returns>A grayscale image represented as Image<L8>.</returns>
        public Image<L8> Apply(Image<Rgba32> original)
        {
            // Clone the original image as a grayscale (L8) image to avoid modifying the original.
            Image<L8> grayscaleImage = original.CloneAs<L8>();

            // Manually iterate over each pixel for custom grayscale conversion.
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    // Get the original pixel color at (x, y).
                    Rgba32 pixelColor = original[x, y];

                    // Compute the grayscale value using the weighted sum of RGB (luminance formula).
                    byte gray = (byte)(0.3 * pixelColor.R + 0.59 * pixelColor.G + 0.11 * pixelColor.B);

                    // Assign the computed grayscale value to the new image.
                    grayscaleImage[x, y] = new L8(gray);
                }
            }

            // Return the processed grayscale image.
            return grayscaleImage;
        }
    }
}