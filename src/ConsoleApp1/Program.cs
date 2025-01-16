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

                // Ensure output directory exists
                //Directory.CreateDirectory(outputFolder);

                // Process all images in the input folder
                foreach (string inputFilePath in Directory.GetFiles(inputFolder, "*.jpg"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    //string outputFilePath = Path.Combine(outputFolder, fileName + ".png");

                    Console.WriteLine(fileName);
                    // Display and process the image
                    DisplayImage(inputFilePath);
                    ProcessImageConversion(inputFilePath, outputFolder);

                }
            }
        }

        private void DisplayImage(string imagePath)
        {
            using (Bitmap image = new Bitmap(imagePath))
            {
                Console.WriteLine($"Displaying image: {imagePath}");
                using (var form = new System.Windows.Forms.Form())
                {
                    form.Text = "Input Image";
                    form.ClientSize = new Size(image.Width, image.Height);

                    var pictureBox = new System.Windows.Forms.PictureBox
                    {
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        Image = image,
                        SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
                    };

                    form.Controls.Add(pictureBox);
                    System.Windows.Forms.Application.Run(form);
                }
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
