namespace OCRProject.ImageProcessing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Imaging.Filters;

public class MedianFiltering
{
    static void Main()
    {
        Bitmap inputImage = new Bitmap("noisy_image.png");

        // Apply Median Filter
        Median filter = new Median(3); // 3x3 kernel
        Bitmap outputImage = filter.Apply(inputImage);

        outputImage.Save("denoised_image.png", ImageFormat.Png);
        Console.WriteLine("Noise reduction applied using Median Filter.");
    }
}