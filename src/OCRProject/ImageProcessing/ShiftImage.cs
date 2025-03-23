using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace OCRProject.ImageProcessing
{
    public class ShiftImage
    {
        /// <summary>
        /// Shifts the image data by the specified X and Y offsets.
        /// </summary>
        /// <param name="image">The input image to be shifted.</param>
        /// <param name="shiftX">The horizontal shift.</param>
        /// <param name="shiftY">The vertical shift.</param>
        /// <returns>A new image with the shifted pixels.</returns>
        public Image<Rgba32> Apply(Image<Rgba32> image, int shiftX, int shiftY)
        {
            // Create a new image with the same dimensions and fill it with white (assuming white background).
            Image<Rgba32> shiftedImage = new Image<Rgba32>(image.Width, image.Height);

            // Define a white pixel (255, 255, 255, 255).
            Rgba32 whitePixel = new Rgba32(255, 255, 255, 255);

            // Initialize the image with white pixels (filling the whole image).
            shiftedImage.Mutate(ctx => ctx.Fill(whitePixel));

            // Loop through each pixel of the input image and shift it.
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Calculate the shifted coordinates
                    int shiftedX = x + shiftX;
                    int shiftedY = y + shiftY;

                    // Debugging: Print out the shifted coordinates before clamping
                    if (shiftedX < 0 || shiftedX >= image.Width || shiftedY < 0 || shiftedY >= image.Height)
                    {
                        Console.WriteLine($"Invalid shifted coordinates: ShiftedX={shiftedX}, ShiftedY={shiftedY} (Original: X={x}, Y={y})");
                    }

                    // Clamp the coordinates to ensure they are within the image bounds
                    shiftedX = Math.Clamp(shiftedX, 0, image.Width - 1);  // Ensure X stays within bounds
                    shiftedY = Math.Clamp(shiftedY, 0, image.Height - 1); // Ensure Y stays within bounds

                    // Debugging: Check if the shifted coordinates are valid after clamping
                    if (shiftedX < 0 || shiftedX >= image.Width || shiftedY < 0 || shiftedY >= image.Height)
                    {
                        Console.WriteLine($"Post-clamp Invalid shifted coordinates: ShiftedX={shiftedX}, ShiftedY={shiftedY} (Original: X={x}, Y={y})");
                    }

                    // Copy the pixel from the original image to the new image at the shifted position
                    shiftedImage[shiftedY, shiftedX] = image[y, x];
                }
            }

            return shiftedImage;
        }
    }
}
