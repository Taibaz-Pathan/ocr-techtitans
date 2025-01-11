using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;

namespace ImageProcessing
{
    public class AdaptiveThresholdProcessor
    {
        public void Process(string inputPath, string outputPath)
        {
            using (Bitmap originalImage = new Bitmap(inputPath))
                {
                    // Convert to grayscale
                    Bitmap grayImage = ConvertToGrayscale(originalImage);

                    // Apply adaptive threshold
                    Bitmap thresholdedImage = ApplyAdaptiveThreshold(grayImage);

                    // Save the processed image
                    thresholdedImage.Save(outputPath, ImageFormat.Png);

                    Console.WriteLine($"Adaptive thresholding applied and saved to {outputPath}");
                }
            
        }

        
    }
}
