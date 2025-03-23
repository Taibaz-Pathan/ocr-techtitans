using System;

namespace OCRProject.ImageProcessing
{
    public class GlobalThresholding
    {
        // Stores the global threshold value for binarization.
        private readonly int _threshold;

        // Constructor: Initializes the GlobalThresholding object with the specified threshold value.
        public GlobalThresholding(int threshold)
        {
            _threshold = threshold;
        }

        // ApplyThreshold: Applies global thresholding to the input image data.
        // Parameters:
        //   imageData: A byte array containing the raw pixel data of the image.
        //   width: The width of the image in pixels.
        //   height: The height of the image in pixels.
        //   bytesPerPixel: The number of bytes per pixel (1 for grayscale, 3 for RGB, 4 for RGBA).
        // Returns:
        //   A byte array containing the thresholded (black and white) image data.
        public byte[] ApplyThreshold(byte[] imageData, int width, int height, int bytesPerPixel)
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

            // Create a new byte array to store the thresholded image data (grayscale, black and white).
            byte[] outputData = new byte[width * height];

            // Loop through each pixel of the image.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the index of the current pixel in the imageData byte array.
                    int pixelIndex = (y * width + x) * bytesPerPixel;

                    // Variable to store the grayscale value of the pixel.
                    int grayscale;

                    // Determine the grayscale value based on the number of bytes per pixel.
                    switch (bytesPerPixel)
                    {
                        case 1: // Grayscale image
                            grayscale = imageData[pixelIndex];
                            break;
                        case 3: // RGB image
                        case 4: // RGBA image
                            grayscale = (imageData[pixelIndex] + imageData[pixelIndex + 1] + imageData[pixelIndex + 2]) / 3;
                            break;
                        default:
                            throw new ArgumentException("Unsupported bytes per pixel.");
                    }

                    // Apply the threshold: Set the pixel to white (255) if the grayscale value is greater than or equal to the threshold,
                    // otherwise set it to black (0).
                    outputData[y * width + x] = grayscale >= _threshold ? (byte)255 : (byte)0;
                }
            }

            // Return the thresholded image data.
            return outputData;
        }
    }
}