using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

        // ApplyThreshold: Applies global thresholding to the input image.
        // Parameters:
        //   image: The Image<Rgba32> object to be thresholded.
        // Returns:
        //   The thresholded Image<Rgba32>.
        public Image<Rgba32> ApplyThreshold(Image<Rgba32> image)
        {
            // Create a new image to store the thresholded data.
            var thresholdedImage = image.Clone();

            // Process the image pixel by pixel.
            thresholdedImage.Mutate(ctx =>
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // Get the current pixel.
                        Rgba32 pixel = image[x, y];

                        // Convert the pixel to grayscale.
                        byte grayscale = (byte)((0.3 * pixel.R) + (0.59 * pixel.G) + (0.11 * pixel.B));

                        // Apply threshold: if the grayscale value is greater than or equal to the threshold, set to white (255), else black (0).
                        byte value = grayscale >= _threshold ? (byte)255 : (byte)0;

                        // Set the pixel to black or white (thresholded).
                        thresholdedImage[x, y] = new Rgba32(value, value, value, 255);
                    }
                }
            });

            return thresholdedImage;
        }
    }
}
