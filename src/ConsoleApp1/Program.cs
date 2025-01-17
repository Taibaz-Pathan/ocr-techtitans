using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;

namespace ImageProcessing
{
    public class AdaptiveThresholdProcessor
    {
        public void ProcessImages()
        {
            {
                // Load configuration using ConfigLoader
                ConfigLoader config = new ConfigLoader();
                string inputFolder = config.InputFolder;
                string outputFolder = config.ExtractedTextFolder;

                Console.WriteLine(inputFolder);

                // Process all images in the input folder
                foreach (string inputFilePath in Directory.GetFiles(inputFolder, "*.jpg"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    //string outputFilePath = Path.Combine(outputFolder, fileName + ".png");

                    Console.WriteLine(fileName);
                    // Display and process the image
                    ShowImage(inputFilePath,"Input Image");
                    ProcessImageConversion(inputFilePath, outputFolder);

                }
            }
        }

        private void ShowImage(object imageSource, string title)
        {
            using (var form = new Form())
            {
                form.Text = title;
                Bitmap image;

                if (imageSource is string imagePath)
                {
                    image = new Bitmap(imagePath);
                }
                else if (imageSource is Bitmap bitmapImage)
                {
                    image = bitmapImage;
                }
                else
                {
                    throw new ArgumentException("Invalid image source.");
                }

                form.ClientSize = new Size(image.Width, image.Height);

                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = image,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                form.Controls.Add(pictureBox);
                Application.Run(form);
            }
        }

        private void ProcessImageConversion(string inputPath, string outputPath)
        {
            try
            {
                // Load the image
                using (Bitmap originalImage = new Bitmap(inputPath))
                {
                    // Convert to grayscale
                    Bitmap grayImage = ConvertToGrayscale(originalImage);
                    ShowImage(grayImage, "Gray scale image");
                }   
            }
            catch (Exception ex)
            {
               
            }
        }

        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayscaleImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    int gray = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);
                    grayscaleImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return grayscaleImage;
        }      

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Adaptive Threshold Processor...");

                // Create an instance of the processor
                AdaptiveThresholdProcessor processor = new AdaptiveThresholdProcessor();

                // Run the processing task
                processor.ProcessImages();

                Console.WriteLine("Processing completed. Press any key to exit.");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Console.ReadKey();
            }
        }

    }
}
