using System;

namespace OCRProject.ImageProcessing
{
    public class SaturationAdjustment
    {
        /// <summary>
        /// Adjusts the saturation of an image.
        /// </summary>
        /// <param name="imageData">The input image data (byte array).</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel.</param>
        /// <param name="saturationFactor">The factor to adjust saturation (1.0 = original, >1.0 = more saturated, <1.0 = less saturated).</param>
        /// <returns>A new byte array with adjusted saturation.</returns>
        public byte[] Apply(byte[] imageData, int width, int height, int bytesPerPixel, float saturationFactor)
        {
            // Input validation: Check for null input data.
            if (imageData == null)
            {
                throw new ArgumentNullException(nameof(imageData), "The input image data cannot be null.");
            }

            // Input validation: Check for invalid image dimensions or bytes per pixel.
            if (width <= 0 || height <= 0 || bytesPerPixel <= 0)
            {
                throw new ArgumentException("Invalid image dimensions or bytes per pixel.");
            }

            // Input validation: Check if the image data length matches the provided dimensions and bytes per pixel.
            if (imageData.Length != width * height * bytesPerPixel)
            {
                throw new ArgumentException("Image data length does not match provided dimensions and bytes per pixel.");
            }

            // Create a new byte array with the same dimensions as the original image data.
            byte[] adjustedData = new byte[imageData.Length];
            Array.Copy(imageData, adjustedData, imageData.Length); // Start with a copy

            // Define the color matrix for saturation adjustment (same as original)
            float[][] colorMatrixElements = {
                new float[] { 0.3086f + 0.6914f * saturationFactor, 0.3086f - 0.3086f * saturationFactor, 0.3086f - 0.3086f * saturationFactor, 0, 0 },
                new float[] { 0.6094f - 0.6094f * saturationFactor, 0.6094f + 0.3906f * saturationFactor, 0.6094f - 0.6094f * saturationFactor, 0, 0 },
                new float[] { 0.0820f - 0.0820f * saturationFactor, 0.0820f - 0.0820f * saturationFactor, 0.0820f + 0.9180f * saturationFactor, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            };

            // Loop through each pixel of the image.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * bytesPerPixel;

                    if (bytesPerPixel >= 3) // Only adjust color pixels
                    {
                        float red = imageData[pixelIndex];
                        float green = imageData[pixelIndex + 1];
                        float blue = imageData[pixelIndex + 2];

                        // Apply the color matrix transformation
                        float newRed = red * colorMatrixElements[0][0] + green * colorMatrixElements[1][0] + blue * colorMatrixElements[2][0];
                        float newGreen = red * colorMatrixElements[0][1] + green * colorMatrixElements[1][1] + blue * colorMatrixElements[2][1];
                        float newBlue = red * colorMatrixElements[0][2] + green * colorMatrixElements[1][2] + blue * colorMatrixElements[2][2];

                        // Clamp the values to the valid range [0, 255]
                        adjustedData[pixelIndex] = (byte)Math.Clamp(newRed, 0, 255);
                        adjustedData[pixelIndex + 1] = (byte)Math.Clamp(newGreen, 0, 255);
                        adjustedData[pixelIndex + 2] = (byte)Math.Clamp(newBlue, 0, 255);
                    }
                    //If RGBA, the alpha channel is copied.
                }
            }

            return adjustedData;
        }
    }
}