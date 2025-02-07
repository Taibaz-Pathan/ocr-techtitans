using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRProject.ImageProcessing
{
    public class ConvertToGrayscale 
    {
        public Bitmap Apply(Bitmap original) 
        {
            Bitmap grayscaleImage = new Bitmap(original.Width, original.Height);

            // Loop through each pixel of the image
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    // Get the color of the current pixel
                    Color originalColor = original.GetPixel(x, y);

                    // Calculate the grayscale value using the luminance formula
                    int gray = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);

                    // Set the pixel color in the grayscale image
                    grayscaleImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return grayscaleImage; //Return the processed grayscale image
        }
    }
}
