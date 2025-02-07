using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRProject.ImageProcessing
{
    public class GlobalThresholding
    {
        private int _threshold;

        public GlobalThresholding(int threshold)
        {
            _threshold = threshold;
        }
        public Bitmap ApplyThreshold(Bitmap image)
        {
            Bitmap outputImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int grayscale = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    Color newColor = grayscale >= _threshold ? Color.White : Color.Black;
                    outputImage.SetPixel(x, y, newColor);
                }
            }

            return outputImage;
        }
    }
}
