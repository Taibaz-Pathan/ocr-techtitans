using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Linq;

namespace OCRProject.ImageProcessing
{
    public class Deskew
    {
        /// <summary>
        /// Applies deskewing to correct image rotation using edge detection and Hough Transform approximation.
        /// </summary>
        public Image<Rgba32> Apply(Image<Rgba32> inputImage)
        {
            try
            {
                // Convert to grayscale
                inputImage.Mutate(x => x.Grayscale());

                // Detect edges using Sobel filter approximation
                var edges = DetectEdges(inputImage);

                // Calculate skew angle
                double skewAngle = CalculateSkewAngle(edges);

                // Rotate image to correct skew
                return RotateImage(inputImage, -skewAngle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Deskew: {ex.Message}");
                return inputImage;
            }
        }

        /// <summary>
        /// Detects edges in the image using a simple Sobel approximation.
        /// </summary>
        private Image<Rgba32> DetectEdges(Image<Rgba32> image)
        {
            var edgeImage = image.Clone();
            edgeImage.Mutate(x => x.DetectEdges()); // Uses built-in ImageSharp filter
            return edgeImage;
        }

        /// <summary>
        /// Calculates the skew angle based on the detected edges.
        /// </summary>
        private double CalculateSkewAngle(Image<Rgba32> edgeImage)
        {
            // Placeholder: Approximate method to determine skew angle from edges.
            // In a more advanced implementation, Hough Transform-like techniques could be used.
            return 0.0; // No skew correction applied yet, needs improvement.
        }

        /// <summary>
        /// Rotates an ImageSharp image by a given angle.
        /// </summary>
        private Image<Rgba32> RotateImage(Image<Rgba32> image, double angle)
        {
            image.Mutate(x => x.Rotate((float)angle));
            return image;
        }
    }
}
