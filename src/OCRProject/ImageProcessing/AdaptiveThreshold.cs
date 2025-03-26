using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace OCRProject.ImageProcessing
{
    public class AdaptiveThreshold
    {
        private readonly int _blockSize;  // Size of the local neighborhood
        private readonly double _c;  // Constant to subtract from the local average (helps fine-tune the threshold)

        // Constructor: Initializes the AdaptiveThreshold object with the block size and constant value.
        public AdaptiveThreshold(int blockSize = 11, double c = 5.0)
        {
            _blockSize = blockSize;
            _c = c;
        }

        // ApplyThreshold: Applies true adaptive thresholding to the input image.
        // Parameters:
        //   image: The image to which the thresholding should be applied.
        // Returns:
        //   A new image with the threshold applied (black and white).
        public Image<Rgba32> ApplyThreshold(Image<Rgba32> image)
        {
            // Clone the image to keep the original intact
            var thresholdedImage = image.Clone();

            // Generate integral image (summed area table)
            var integralImage = GenerateIntegralImage(image);

            // Loop through every pixel in the image
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Get the grayscale value for the current pixel
                    byte grayscale = GetGrayscaleValue(image[x, y]);

                    // Calculate the local threshold using the integral image
                    byte localThreshold = CalculateLocalThreshold(integralImage, x, y, image.Width, image.Height);

                    // Apply the threshold: if the grayscale value is below the local threshold, set to black, else white.
                    if (grayscale < localThreshold)
                    {
                        thresholdedImage[x, y] = new Rgba32(0, 0, 0, 255); // Black
                    }
                    else
                    {
                        thresholdedImage[x, y] = new Rgba32(255, 255, 255, 255); // White
                    }
                }
            }

            return thresholdedImage;
        }

        // GetGrayscaleValue: Converts a pixel to its grayscale value (using luminance formula).
        private byte GetGrayscaleValue(Rgba32 pixel)
        {
            return (byte)((0.3 * pixel.R) + (0.59 * pixel.G) + (0.11 * pixel.B));
        }

        // GenerateIntegralImage: Generates an integral image (summed area table) for fast block sum computation.
        private int[,] GenerateIntegralImage(Image<Rgba32> image)
        {
            int width = image.Width;
            int height = image.Height;

            // Create an integral image (summed area table)
            int[,] integralImage = new int[height + 1, width + 1];

            // Fill the integral image with cumulative sum of grayscale values
            for (int y = 1; y <= height; y++)
            {
                for (int x = 1; x <= width; x++)
                {
                    int pixelValue = GetGrayscaleValue(image[x - 1, y - 1]);
                    integralImage[y, x] = pixelValue + integralImage[y - 1, x] + integralImage[y, x - 1] - integralImage[y - 1, x - 1];
                }
            }

            return integralImage;
        }

        // CalculateLocalThreshold: Calculates the local threshold for a pixel based on its neighborhood using the integral image.
        private byte CalculateLocalThreshold(int[,] integralImage, int x, int y, int width, int height)
        {
            int halfBlockSize = _blockSize / 2;

            // Calculate the boundaries of the block centered at (x, y)
            int x1 = Math.Max(x - halfBlockSize, 0);
            int y1 = Math.Max(y - halfBlockSize, 0);
            int x2 = Math.Min(x + halfBlockSize, width - 1);
            int y2 = Math.Min(y + halfBlockSize, height - 1);

            // Get the sum of the block using the integral image
            int sum = integralImage[y2 + 1, x2 + 1] - integralImage[y1, x2 + 1] - integralImage[y2 + 1, x1] + integralImage[y1, x1];

            // Calculate the number of pixels in the block
            int blockArea = (x2 - x1 + 1) * (y2 - y1 + 1);

            // Calculate the local threshold as the mean of the block minus a constant
            byte localThreshold = (byte)((sum / blockArea) - _c);
            return localThreshold;
        }
    }
}
