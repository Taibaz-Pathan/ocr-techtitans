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
        private int thresholdValue;

        public Bitmap ApplyThreshold(Bitmap original)
        {
            //using (var image = (Bitmap)Image.FromFile(inputPath))
            Bitmap image = new Bitmap(original.Width, original.Height);
            {
                var thresholdedImage = new Bitmap(image.Width, image.Height);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixel = image.GetPixel(x, y);

                        int grayscale = (pixel.R + pixel.G + pixel.B) / 3;

                        thresholdedImage.SetPixel(x, y, grayscale < thresholdValue ? Color.Black : Color.White);
                    }

                }
                //thresholdedImage.Save(outputPath, ImageFormat.Png);
                return thresholdedImage;
            }
        }
    }
}
