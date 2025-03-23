using System;

namespace OCRProject.ImageProcessing
{
    public class AdaptiveThreshold
    {
        // Stores the threshold value used for binarization.
        private readonly int _thresholdValue;

        // Constructor: Initializes the AdaptiveThreshold object with the specified threshold value.
        public AdaptiveThreshold(int thresholdValue)
        {
            _thresholdValue = thresholdValue;
        }

        // ApplyThreshold: Performs adaptive thresholding on the input image data.
        // Parameters:
        //   originalData: A byte array containing the raw pixel data of the image.
        //   width: The width of the image in pixels.
        //   height: The height of the image in pixels.
        //   bytesPerPixel: The number of bytes per pixel (1 for grayscale, 3 for RGB, 4 for RGBA).
        // Returns:
        //   A byte array containing the thresholded (black and white) grayscale image data.
        public byte[] ApplyThreshold(byte[] originalData, int width, int height, int bytesPerPixel)
        {
            // Input validation: Check for null input data.
            if (originalData == null)
            {
                throw new ArgumentNullException(nameof(originalData), "The input image data cannot be null.");
            }

            // Input validation: Check for invalid image dimensions or bytes per pixel.
            if (width <= 0 || height <= 0 || bytesPerPixel <= 0)
            {
                throw new ArgumentException("Invalid image dimensions or bytes per pixel.");
            }

            // Input validation: Check if the image data length matches the provided dimensions and bytes per pixel.
            if (originalData.Length != width * height * bytesPerPixel)
            {
                throw new ArgumentException("Image data length does not match provided dimensions and bytes per pixel.");
            }

            // Create a new byte array to store the thresholded grayscale image data.
            // Each pixel will be represented by a single byte (0 for black, 255 for white).
            byte[] thresholdedData = new byte[width * height];

            // Iterate through each pixel of the image.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Calculate the index of the current pixel in the originalData byte array.
                    int pixelIndex = (y * width + x) * bytesPerPixel;

                    // Variable to store the grayscale value of the pixel.
                    int grayscale;

                    // Determine the grayscale value based on the number of bytes per pixel.
                    switch (bytesPerPixel)
                    {
                        case 1: // Grayscale image: The pixel value is already the grayscale value.
                            grayscale = originalData[pixelIndex];
                            break;                        
                        case 2: // RGBA image: Calculate the average of the red, green, and blue components.
                            grayscale = (originalData[pixelIndex] + originalData[pixelIndex + 1] + originalData[pixelIndex + 2]) / 3;
                            break;
                        default: // Unsupported number of bytes per pixel.
                            throw new ArgumentException("Unsupported bytes per pixel.");
                    }

                    // Apply the threshold: Set the pixel to black (0) if the grayscale value is below the threshold,
                    // otherwise set it to white (255).
                    thresholdedData[y * width + x] = grayscale < _thresholdValue ? (byte)0 : (byte)255;
                }
            }

            // Return the thresholded grayscale image data.
            return thresholdedData;
        }
    }
}