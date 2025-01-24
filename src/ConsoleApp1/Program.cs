using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;
using Tesseract; // Ensure Tesseract NuGet package is installed


namespace ImageProcessing
{
    public class AdaptiveThresholdProcessor
    {
        public void ProcessImages()
        {
            {
                // Load configuration using ConfigLoader
                ConfigLoader config = new ConfigLoader();

                // Folder containing input images
                string inputFolder = config.InputFolder;

                // Folder for storing processed images or results
                string outputFolder = config.ExtractedTextFolder;

                Console.WriteLine(inputFolder);

                // Process all images in the input folder
                foreach (string inputFilePath in Directory.GetFiles(inputFolder, "*.jpg"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    //string outputFilePath = Path.Combine(outputFolder, fileName + ".png");

                    // Display the image
                    ShowImage(inputFilePath,"Input Image");

                    //Process the image 
                    ProcessImageConversion(inputFilePath, outputFolder);

                    //Extract text
                    ExtractTextFromImage(thresholdedImage);

                }
            }
        }

        // Method to display an image in a Windows Form
        private void ShowImage(object imageSource, string title)
        {
            using (var form = new Form()) // Create a new form
            {
                form.Text = title;
                Bitmap image;

                // Determine the source of the image (file path or Bitmap object)
                if (imageSource is string imagePath)
                {
                    image = new Bitmap(imagePath);
                }
                else if (imageSource is Bitmap bitmapImage)
                {
                    image = bitmapImage; // Use the Bitmap object directly
                }
                else
                {
                    throw new ArgumentException("Invalid image source."); // Handle invalid input
                }

                form.ClientSize = new Size(image.Width, image.Height); // Adjust the form size to match the image dimensions

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

        // Method to convert an image to grayscale and display it
        private void ProcessImageConversion(string inputPath, string outputPath)
        {
            try
            {
                // Load the image
                using (Bitmap originalImage = new Bitmap(inputPath))
                {
                    // Convert to grayscale
                    Bitmap grayImage = ConvertToGrayscale(originalImage);                    

                    // Apply adaptive threshold
                    Bitmap thresholdedImage = ApplyAdaptiveThreshold(grayImage);
                    ShowImage(thresholdedImage, "Threshold image");
                }   

            }
            catch (Exception ex)
            {
               
            }
        }

        // Method to convert a color image to grayscale
        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayscaleImage = new Bitmap(original.Width, original.Height);

            // Loop through each pixel of the image
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    // Get the color of the current pixel
                    Color originalColor = original.GetPixel(x, y);

                    // Calculate the grayscale value using the luminance formula
                    int gray = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);

                    // Set the pixel color in the grayscale image
                    grayscaleImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return grayscaleImage;
        }

        private Bitmap ApplyAdaptiveThreshold(Bitmap grayscaleImage)
        {
            Bitmap thresholdedImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);

            int blockSize = 11; // Size of the block used for thresholding
            int threshold = 127; // Initial threshold value

            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    int sum = 0;
                    int count = 0;

                    // Calculate the average intensity within the block
                    for (int by = -blockSize / 2; by <= blockSize / 2; by++)
                    {
                        for (int bx = -blockSize / 2; bx <= blockSize / 2; bx++)
                        {
                            int nx = x + bx;
                            int ny = y + by;

                            if (nx >= 0 && nx < grayscaleImage.Width && ny >= 0 && ny < grayscaleImage.Height)
                            {
                                Color pixelColor = grayscaleImage.GetPixel(nx, ny);
                                sum += pixelColor.R; // Red channel intensity
                                count++;
                            }

                        }
                    }

                    int averageIntensity = sum / count;
                    // Apply threshold
                    if (averageIntensity > threshold)
                    {
                        thresholdedImage.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        thresholdedImage.SetPixel(x, y, Color.Black);
                    }

                }
            }

            return thresholdedImage;
        }

        private void ExtractTextFromImage(Bitmap image)
        {
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var page = engine.Process(image))
                    {
                        string extractedText = page.GetText();
                        Console.WriteLine($"Extracted text is"+ extractedText);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
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
