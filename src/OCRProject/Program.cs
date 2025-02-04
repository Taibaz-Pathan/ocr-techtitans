using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;
using Tesseract; // Ensure Tesseract NuGet package is installed
using System.Windows.Forms;
using OCRProject.ImageProcessing;


class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Starting Image Processing...");

            // Load configuration
            ConfigLoader config = new ConfigLoader();
            string inputFolder = config.InputFolder;
            string outputFolder = config.ExtractedTextFolder;

            // Ensure output folder exists
            Directory.CreateDirectory(outputFolder);

            // Process all images in the input folder
            foreach (string inputFilePath in Directory.GetFiles(inputFolder, "*.png"))
            {
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string outputGrayscalePath = Path.Combine(outputFolder, fileName + "_grayscale.png");
                string outputThresholdPath = Path.Combine(outputFolder, fileName + "_threshold.png");

                // Load the image
                using (Bitmap inputImage = new Bitmap(inputFilePath))
                {
                    ShowImage(inputImage, "Original Image");

                    // Convert to grayscale
                    ConvertToGrayscale grayscaleConverter = new ConvertToGrayscale();
                    Bitmap grayImage = grayscaleConverter.Apply(inputImage);
                    ShowImage(grayImage, "Grayscale Image");
                    grayImage.Save(outputGrayscalePath, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine($"Grayscale image saved: {outputGrayscalePath}");

                    // Apply adaptive thresholding
                    AdaptiveThreshold adaptiveThreshold = new AdaptiveThreshold();
                    Bitmap adaptiveThresholdImage = adaptiveThreshold.ApplyThreshold(inputImage);
                    ShowImage(adaptiveThresholdImage, "Adaptive Threshold Image");
                    //adaptiveThresholdImage.Save(outputThresholdPath, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine($"Adaptive threshold image saved: {outputThresholdPath}");

                    // Apply global thresholding (Example: threshold = 128)
                    GlobalThresholding globalThresholding = new GlobalThresholding(128);
                    Bitmap globalThresholdImage = globalThresholding.ApplyThreshold(inputImage);
                    //string outputGlobalThresholdPath = Path.Combine(outputFolder, fileName + "_global_threshold.png");
                    ShowImage(globalThresholdImage, "Global Threshold Image");
                    //globalThresholdImage.Save(outputGlobalThresholdPath, System.Drawing.Imaging.ImageFormat.Png);
                    //Console.WriteLine($"Global threshold image saved: {outputGlobalThresholdPath}");

                    // Extract text using OCR (Tesseract)
                    ExtractTextFromImage(globalThresholdImage);
                }
            }

            Console.WriteLine("Processing completed. Press any key to exit.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Process: {ex.Message}");
        }
        finally
        {
            Console.ReadKey();
        }
    }

    // Method to display an image in a Windows Form
    private static void ShowImage(Bitmap image, string title)
    {
        using (var form = new Form())
        {
            form.Text = title;
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

    private static void ExtractTextFromImage(Bitmap image)
    {
        try
        {
            using (var stream = new MemoryStream())
            {
                // Save Bitmap as a TIFF in memory
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                stream.Position = 0;

                // Load the Pix from the memory stream
                using (var pixImage = Pix.LoadTiffFromMemory(stream.ToArray()))
                {
                    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                    {
                        using (var page = engine.Process(pixImage))
                        {
                            string extractedText = page.GetText();
                            Console.WriteLine($"Extracted text: {extractedText}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ExtractTextFromImage: {ex.Message}");
        }
    }
}