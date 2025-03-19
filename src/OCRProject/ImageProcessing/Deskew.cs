using System;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace OCRProject.ImageProcessing
{
    public class Deskew
    {
        /// <summary>
        /// Applies deskewing to correct image rotation using Hough Transform.
        /// </summary>
        public Image<Rgba32> Apply(Image<Rgba32> inputImage)
        {
            try
            {
                // Convert ImageSharp image to OpenCV Mat
                Mat grayMat = ConvertToMat(inputImage);

                // Apply Canny edge detection
                Mat edges = new Mat();
                Cv2.Canny(grayMat, edges, 50, 150, 3);

                // Apply Hough Line Transform
                LineSegmentPolar[] lines = Cv2.HoughLines(edges, 1, Math.PI / 180, 100);

                if (lines.Length == 0)
                    return inputImage; // No skew detected

                // Compute the average skew angle
                double angleSum = 0;
                int count = 0;
                foreach (var line in lines)
                {
                    double theta = line.Theta * (180 / Math.PI); // Convert radians to degrees
                    if (theta > 45 && theta < 135) // Filter out irrelevant angles
                    {
                        angleSum += theta - 90; // Convert to rotation angles
                        count++;
                    }
                }

                if (count == 0)
                    return inputImage; // No valid lines detected

                double skewAngle = angleSum / count;

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
        /// Rotates an image by the given angle.
        /// </summary>
        private Image<Rgba32> RotateImage(Image<Rgba32> image, double angle)
        {
            image.Mutate(x => x.Rotate((float)angle));
            return image;
        }

        /// <summary>
        /// Converts an ImageSharp image to an OpenCV Mat.
        /// </summary>
        private Mat ConvertToMat(Image<Rgba32> image)
        {
            int width = image.Width;
            int height = image.Height;
            byte[] grayscaleData = new byte[width * height];

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        grayscaleData[y * width + x] = row[x].R; // Use Red channel as grayscale
                    }
                }
            });

            // Create a Mat manually from grayscale data
            Mat mat = new Mat(height, width, MatType.CV_8UC1);
            Marshal.Copy(grayscaleData, 0, mat.Data, grayscaleData.Length);
            return mat;
        }
    }
}
