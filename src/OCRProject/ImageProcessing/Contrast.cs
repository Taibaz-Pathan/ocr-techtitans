using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace OCRProject.ImageProcessing
{
    public class Contrast
    {
        public Bitmap AdjustContrast(Bitmap image, float contrast)
        {
            contrast = (100.0f + contrast) / 100.0f;
            contrast *= contrast;

            Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(adjustedImage))
            {
                ImageAttributes attributes = new ImageAttributes();
                float[][] colorMatrixElements = {
                    new float[] {contrast, 0, 0, 0, 0},
                    new float[] {0, contrast, 0, 0, 0},
                    new float[] {0, 0, contrast, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0.5f * (1.0f - contrast), 0.5f * (1.0f - contrast), 0.5f * (1.0f - contrast), 0, 1}
                };
                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }

            return adjustedImage;
        }
    }
}