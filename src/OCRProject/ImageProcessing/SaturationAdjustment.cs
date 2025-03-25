using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace OCRProject.ImageProcessing
{
    public class SaturationAdjustment
    {
        /// <summary>
        /// Adjusts the saturation of an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="saturationFactor">The factor to adjust saturation (1.0 = original, >1.0 = more saturated, <1.0 = less saturated).</param>
        /// <returns>A new image with adjusted saturation.</returns>
        public Image<Rgba32> Apply(Image<Rgba32> image, float saturationFactor)
        {
            // Clone the image to avoid modifying the original
            var adjustedImage = image.Clone();

            // Define the color matrix for saturation adjustment (same as original)
            float[][] colorMatrixElements = {
                new float[] { 0.3086f + 0.6914f * saturationFactor, 0.3086f - 0.3086f * saturationFactor, 0.3086f - 0.3086f * saturationFactor, 0, 0 },
                new float[] { 0.6094f - 0.6094f * saturationFactor, 0.6094f + 0.3906f * saturationFactor, 0.6094f - 0.6094f * saturationFactor, 0, 0 },
                new float[] { 0.0820f - 0.0820f * saturationFactor, 0.0820f - 0.0820f * saturationFactor, 0.0820f + 0.9180f * saturationFactor, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            };

            // Loop through each pixel of the image.
            for (int y = 0; y < adjustedImage.Height; y++)
            {
                for (int x = 0; x < adjustedImage.Width; x++)
                {
                    Rgba32 pixel = adjustedImage[x, y];

                    // Apply the color matrix transformation
                    float newRed = pixel.R * colorMatrixElements[0][0] + pixel.G * colorMatrixElements[1][0] + pixel.B * colorMatrixElements[2][0];
                    float newGreen = pixel.R * colorMatrixElements[0][1] + pixel.G * colorMatrixElements[1][1] + pixel.B * colorMatrixElements[2][1];
                    float newBlue = pixel.R * colorMatrixElements[0][2] + pixel.G * colorMatrixElements[1][2] + pixel.B * colorMatrixElements[2][2];

                    // Clamp the values to the valid range [0, 255]
                    adjustedImage[x, y] = new Rgba32(
                        (byte)Math.Clamp(newRed, 0, 255),
                        (byte)Math.Clamp(newGreen, 0, 255),
                        (byte)Math.Clamp(newBlue, 0, 255),
                        pixel.A // Keep the alpha value unchanged
                    );
                }
            }

            return adjustedImage;
        }
    }
}
