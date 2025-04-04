using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

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
        // Validate image dimensions
        if (image.Width <= 0 || image.Height <= 0)
        {
            throw new InvalidOperationException("Image dimensions are invalid.");
        }

        // Create a new image with the same dimensions and fill it with white (assuming white background).
        Image<Rgba32> shiftedImage = new Image<Rgba32>(image.Width, image.Height);

        // Define a white pixel (255, 255, 255, 255).
        Rgba32 whitePixel = new Rgba32(255, 255, 255, 255);

        // Initialize the image with white pixels (filling the whole image).
        shiftedImage.Mutate(ctx => ctx.Fill(whitePixel));

        // Log the shift values
        Console.WriteLine($"Applying Shift: X={shiftX}, Y={shiftY}");

        // Handle negative shifts by adjusting the range
        int maxShiftX = shiftX;
        int maxShiftY = shiftY;

        // Loop through each pixel of the input image and shift it.
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                // Calculate the shifted coordinates
                int shiftedX = x + maxShiftX;
                int shiftedY = y + maxShiftY;

                // If the shifted coordinates are out of bounds (negative or too large), skip that pixel
                if (shiftedX < 0 || shiftedX >= image.Width || shiftedY < 0 || shiftedY >= image.Height)
                {
                    continue; // Skip pixels that are out of bounds
                }

                // If the shifted coordinates are valid, copy the pixel to the new image
                shiftedImage[shiftedY, shiftedX] = image[y, x];
            }
        }

        return shiftedImage;
    }
}

