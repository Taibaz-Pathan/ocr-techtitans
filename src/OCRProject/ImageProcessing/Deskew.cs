using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using AForge.Imaging.Filters; 
using AForge.Imaging;

namespace OCRProject.ImageProcessing
{
    public class Deskew
    {
        /// <summary>
        /// Applies deskewing to correct image rotation
        /// </summary>
        /// <param name="inputImage">The input image</param>
        /// <returns>Deskewed image</returns>
        public Bitmap Apply(Bitmap inputImage)
        {
            try
            {
                // Convert to grayscale
                Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayImage = grayscaleFilter.Apply(inputImage);

                // Apply Hough Line Transform to detect skew angle
                HoughLineTransformation houghTransform = new HoughLineTransformation();
                houghTransform.ProcessImage(grayImage);
                HoughLine[] lines = houghTransform.GetLinesByRelativeIntensity(0.5);

                if (lines.Length == 0)
                    return inputImage; // No skew detected

                double angleSum = 0;
                int count = 0;

                foreach (HoughLine line in lines)
                {
                    double theta = line.Theta;
                    if (theta > 45 && theta < 135)
                    {
                        angleSum += theta - 90;
                        count++;
                    }
                }

                if (count == 0)
                    return inputImage;

                double skewAngle = angleSum / count;
                return RotateImage(inputImage, -skewAngle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Deskew: {ex.Message}");
                return inputImage;
            }
        }

        /// <summary>
        /// Rotates an image by a given angle
        /// </summary>
        private Bitmap RotateImage(Bitmap inputImage, double angle)
        {
            Bitmap rotatedImage = new Bitmap(inputImage.Width, inputImage.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TranslateTransform(inputImage.Width / 2, inputImage.Height / 2);
                g.RotateTransform((float)angle);
                g.TranslateTransform(-inputImage.Width / 2, -inputImage.Height / 2);
                g.DrawImage(inputImage, new Point(0, 0));
            }
            return rotatedImage;
        }
    }
}
