using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace OCRProject.ImageProcessing
{
    public class AdaptiveThreshold
    {
        // Threshold value used to decide whether a pixel should be black or white.
        private int thresholdValue;

        /// <summary>
        /// Applies a simple adaptive thresholding technique to a given image.
        /// Converts pixels to either black or white based on their grayscale value.
        /// </summary>
        /// <param name="original">The input bitmap image</param>
        /// <returns>A thresholded bitmap image</returns>
        public Bitmap ApplyThreshold(Bitmap original)
        {
            // Create a new bitmap with the same dimensions as the original image.
            Bitmap image = new Bitmap(original.Width, original.Height);
            
            // Create another bitmap to store the thresholded result.
            var thresholdedImage = new Bitmap(image.Width, image.Height);

            // Iterate through each pixel in the image.
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    // Get the pixel color at the current (x, y) coordinate.
                    var pixel = image.GetPixel(x, y);

                    // Convert the pixel to grayscale using an average of RGB components.
                    int grayscale = (pixel.R + pixel.G + pixel.B) / 3;

                    // Apply thresholding: if grayscale value is below threshold, set to black; otherwise, set to white.
                    thresholdedImage.SetPixel(x, y, grayscale < thresholdValue ? Color.Black : Color.White);
                }
            }

            // Return the processed thresholded image.
            return thresholdedImage;
        }
    }
}