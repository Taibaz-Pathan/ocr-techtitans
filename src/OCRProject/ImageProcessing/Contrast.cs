using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OCRProject.ImageProcessing
{
    public class Contrast
    {
        public Bitmap AdjustContrast(Bitmap image, float contrast)
        {
            contrast = (100.0f + contrast) / 100.0f;
            contrast *= contrast;

            Bitmap adjustedImage = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData adjData = adjustedImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytes = Math.Abs(imageData.Stride) * image.Height;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(imageData.Scan0, buffer, 0, bytes);
            image.UnlockBits(imageData);

            for (int i = 0; i < buffer.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    float color = buffer[i + j] / 255.0f;
                    color -= 0.5f;
                    color *= contrast;
                    color += 0.5f;
                    color *= 255;
                    buffer[i + j] = (byte)Math.Max(0, Math.Min(255, color));
                }
            }

            Marshal.Copy(buffer, 0, adjData.Scan0, bytes);
            adjustedImage.UnlockBits(adjData);
            return adjustedImage;
        }
    }
}