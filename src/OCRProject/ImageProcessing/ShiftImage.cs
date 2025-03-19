using System;

namespace OCRProject.ImageProcessing
{
    public class ShiftImage
    {
        /// <summary>
        /// Shifts the image data by the specified X and Y offsets.
        /// </summary>
        /// <param name="imageData">The input image data (byte array).</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel.</param>
        /// <param name="shiftX">The horizontal shift.</param>
        /// <param name="shiftY">The vertical shift.</param>
        /// <returns>A new byte array with the shifted image data.</returns>
        public byte[] Apply(byte[] imageData, int width, int height, int bytesPerPixel, int shiftX, int shiftY)
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

            // Create a new byte array to store the shifted image data.
            byte[] shiftedData = new byte[imageData.Length];

            // Initialize the shifted data with white pixels (assuming white background).
            for (int i = 0; i < shiftedData.Length; i++)
            {
                if (bytesPerPixel == 1) // Grayscale
                {
                    shiftedData[i] = 255; // White
                }
                else if (bytesPerPixel == 3) // RGB
                {
                    if (i % 3 == 0) shiftedData[i] = 255;
                    if (i % 3 == 1) shiftedData[i] = 255;
                    if (i % 3 == 2) shiftedData[i] = 255;
                }
                else if (bytesPerPixel == 4) // RGBA
                {
                    if (i % 4 == 0) shiftedData[i] = 255;
                    if (i % 4 == 1) shiftedData[i] = 255;
                    if (i % 4 == 2) shiftedData[i] = 255;
                    if (i % 4 == 3) shiftedData[i] = 255;
                }
            }

            // Loop through the original image data and copy pixels to the shifted position.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the shifted coordinates.
                    int shiftedX = x + shiftX;
                    int shiftedY = y + shiftY;

                    // Check if the shifted pixel is within the image bounds.
                    if (shiftedX >= 0 && shiftedX < width && shiftedY >= 0 && shiftedY < height)
                    {
                        // Calculate the index of the original pixel.
                        int originalIndex = (y * width + x) * bytesPerPixel;
                        // Calculate the index of the shifted pixel.
                        int shiftedIndex = (shiftedY * width + shiftedX) * bytesPerPixel;

                        // Copy the pixel data.
                        for (int i = 0; i < bytesPerPixel; i++)
                        {
                            shiftedData[shiftedIndex + i] = imageData[originalIndex + i];
                        }
                    }
                }
            }

            // Return the shifted image data.
            return shiftedData;
        }
    }
}