using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OCRProject.ImageProcessing
{
    public class Binarization
    {
        public Bitmap ApplyBinarization(Bitmap image, float threshold)
        {
            Bitmap binarizedImage = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData binData = binarizedImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytes = Math.Abs(imageData.Stride) * image.Height;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(imageData.Scan0, buffer, 0, bytes);
            image.UnlockBits(imageData);

            for (int i = 0; i < buffer.Length; i += 3)
            {
                int gray = (int)(0.299 * buffer[i + 2] + 0.587 * buffer[i + 1] + 0.114 * buffer[i]);
                byte binColor = (byte)(gray < (threshold * 255) ? 0 : 255);
                buffer[i] = buffer[i + 1] = buffer[i + 2] = binColor;
            }

            Marshal.Copy(buffer, 0, binData.Scan0, bytes);
            binarizedImage.UnlockBits(binData);
            return binarizedImage;
        }
    }
}