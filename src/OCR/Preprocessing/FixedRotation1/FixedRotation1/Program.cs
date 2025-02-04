using System;
using System.IO;
using OpenCvSharp;  // For OpenCV (Auto Alignment)
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace FixedRotationTest
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Loading image...");
                string imagePath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Input/AlignmentTest.jpeg";
                string fixedRotationPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/auto_align90.jpg";
                string autoAlignPath = @"/Users/khushalsingh/Downloads/ocr-techtitans/Output/auto_align.jpg";

                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Image file not found!");
                    return;
                }

                Console.WriteLine("\nChoose Rotation Mode:");
                Console.WriteLine("1. Fixed 90-degree Rotation");
                Console.WriteLine("2. Auto Alignment (Correct Skew)");
                Console.Write("Enter option (1/2): ");
                string? choice = Console.ReadLine();
                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                    return;
                }


                if (choice == "1")
                {
                    PreprocessingManager.RotateImageFixed(imagePath, fixedRotationPath, 90);
                    Console.WriteLine($"Rotated image saved at: {fixedRotationPath}");
                }
                else if (choice == "2")
                {
                    PreprocessingManager.AutoAlignImage(imagePath, autoAlignPath);
                    Console.WriteLine($"Auto-aligned image saved at: {autoAlignPath}");
                }
                else
                {
                    Console.WriteLine("Invalid option. Please enter 1 or 2.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.ToString());
            }
        }
    }

    static class PreprocessingManager
    {
        /// <summary>
        /// Rotates an image by a fixed angle (e.g., 90 degrees).
        /// </summary>
        public static void RotateImageFixed(string inputPath, string outputPath, float angle)
        {
            using (Image image = Image.Load(inputPath))
            {
                image.Mutate(x => x.Rotate(angle));
                image.Save(outputPath, new JpegEncoder());
            }
        }

        /// <summary>
        /// Auto-aligns an image by detecting skew and correcting it.
        /// </summary>
        public static void AutoAlignImage(string inputPath, string outputPath)
        {
            Mat image = Cv2.ImRead(inputPath, ImreadModes.Grayscale);
            Mat edges = new Mat();
            Cv2.Canny(image, edges, 50, 150);

            LineSegmentPolar[] lines = Cv2.HoughLines(edges, 1, Math.PI / 180, 200);
            double detectedAngle = 0;
            
            if (lines.Length > 0)
            {
                detectedAngle = lines[0].Theta * (180 / Math.PI); // Convert radians to degrees
            }

            RotateImageFixed(inputPath, outputPath, (float)-detectedAngle);

        }
    }
}