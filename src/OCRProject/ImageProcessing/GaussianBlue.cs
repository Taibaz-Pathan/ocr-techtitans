using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Bmp;

namespace OCRProject.ImageProcessing
{
    public class GaussianBlur
    {
        public Bitmap Apply(Bitmap image, float blurIntensity = 1.5f)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Convert Bitmap to ImageSharp format
                image.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;

                using (var imgSharp = SixLabors.ImageSharp.Image.Load(memoryStream))
                {
                    imgSharp.Mutate(x => x.GaussianBlur(blurIntensity));

                    // Convert ImageSharp back to Bitmap
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        imgSharp.Save(outputStream, new BmpEncoder());
                        outputStream.Position = 0;
                        return new Bitmap(outputStream);
                    }
                }
            }
        }
    }
}