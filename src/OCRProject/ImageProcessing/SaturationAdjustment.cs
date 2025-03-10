using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRProject.ImageProcessing
{
    public class SaturationAdjustment
    {
        /// <summary>
        /// Adjusts the saturation of an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="saturationFactor">The factor to adjust saturation (1.0 = original, >1.0 = more saturated, <1.0 = less saturated).</param>
        /// <returns>A new Bitmap with adjusted saturation.</returns>
        public Bitmap Apply(Bitmap image, float saturationFactor)
        {
            // Create a new bitmap with the same dimensions as the original image
            Bitmap adjustedBitmap = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(adjustedBitmap))
            {
                // Define the color matrix for saturation adjustment
                float[][] colorMatrixElements = {
                    new float[] { 0.3086f + 0.6914f * saturationFactor, 0.3086f - 0.3086f * saturationFactor, 0.3086f - 0.3086f * saturationFactor, 0, 0 },
                    new float[] { 0.6094f - 0.6094f * saturationFactor, 0.6094f + 0.3906f * saturationFactor, 0.6094f - 0.6094f * saturationFactor, 0, 0 },
                    new float[] { 0.0820f - 0.0820f * saturationFactor, 0.0820f - 0.0820f * saturationFactor, 0.0820f + 0.9180f * saturationFactor, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                // Create a ColorMatrix object with the defined matrix
                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    // Apply the color matrix to the image
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return adjustedBitmap;
        }
    }
}
