using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;
using Tesseract; // Ensure Tesseract NuGet package is installed
using System.Windows.Forms;
using OCRProject.ImageProcessing;
using OCRProject.Utils;
using System.IO.Pipelines;


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
            string outputFolderImage = config.OutputImageFolder;
            string outputFolderText = config.ExtractedTextFolder;

            // Create an instance of CreateFiles
            CreateFiles fileCreator = new CreateFiles();
            string fileNameExtracted = "ProcessedFile_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            // Call the CreateTextFile method
            string createdFilePath = fileCreator.CreateTextFile(outputFolderText, fileNameExtracted); 

            if (createdFilePath != null)
            {
                Console.WriteLine($"Text file created successfully: {createdFilePath}");
            }
            else
            {
                Console.WriteLine("Failed to create text file.");
            }

            FileWriter fileWriter = new FileWriter();

            // Process all images in the input folder
            foreach (string inputFilePath in Directory.GetFiles(inputFolder, "*.png"))
            {
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string outputGrayscalePath = Path.Combine(outputFolderImage, fileName + "_grayscale.png");
                string outputThresholdPath = Path.Combine(outputFolderImage, fileName + "_Adaptivethreshold.png");

                // Load the image
                using (Bitmap inputImage = new Bitmap(inputFilePath))
                {
                    ShowImage(inputImage, "Original Image");

                    // Convert to grayscale
                    ConvertToGrayscale grayscaleConverter = new ConvertToGrayscale();
                    Bitmap grayImage = grayscaleConverter.Apply(inputImage);
                    //ShowImage(grayImage, "Grayscale Image");
                    grayImage.Save(Path.Combine(outputFolderImage, fileName + "_grayscale.png"), System.Drawing.Imaging.ImageFormat.Png);
                    ExtractTextFromImage(grayImage, createdFilePath, fileWriter);

                    // Apply adaptive thresholding
                    AdaptiveThreshold adaptiveThreshold = new AdaptiveThreshold();
                    Bitmap adaptiveThresholdImage = adaptiveThreshold.ApplyThreshold(inputImage);
                    //ShowImage(adaptiveThresholdImage, "Adaptive Threshold Image");
                    adaptiveThresholdImage.Save(Path.Combine(outputFolderImage, fileName + "_AdaptiveThreshold.png"), System.Drawing.Imaging.ImageFormat.Png);
                    ExtractTextFromImage(adaptiveThresholdImage, createdFilePath, fileWriter);

                    // Apply global thresholding (Example: threshold = 128)
                    GlobalThresholding globalThresholding = new GlobalThresholding(128);
                    Bitmap globalThresholdImage = globalThresholding.ApplyThreshold(inputImage);
                    //ShowImage(globalThresholdImage, "Global Threshold Image");
                    globalThresholdImage.Save(Path.Combine(outputFolderImage, fileName + "globalThreshold.png"), System.Drawing.Imaging.ImageFormat.Png);
                    ExtractTextFromImage(globalThresholdImage, createdFilePath, fileWriter);

                    ShiftImage shiftProcessor = new ShiftImage();
                    Bitmap shiftedImage = shiftProcessor.Apply(inputImage, 5, 5);
                    //ShowImage(shiftedImage, "Shifted Image");
                    shiftedImage.Save(Path.Combine(outputFolderImage, fileName + "_shiftedImage.png"), System.Drawing.Imaging.ImageFormat.Png);
                    ExtractTextFromImage(shiftedImage, createdFilePath, fileWriter);

                    SaturationAdjustment saturationProcessor = new SaturationAdjustment();
                    Bitmap adjustedImage = saturationProcessor.Apply(globalThresholdImage, 1.2f); // Adjust saturation factor
                    //ShowImage(adjustedImage, "Saturation Adjusted Image");
                    shiftedImage.Save(Path.Combine(outputFolderImage, fileName + "_SaturationAjusted.png"), System.Drawing.Imaging.ImageFormat.Png);
                    ExtractTextFromImage(adjustedImage, createdFilePath, fileWriter);

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

    private static void ExtractTextFromImage(Bitmap image, string createdFilePath, FileWriter fileWriter)
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

                            // Write extracted text to the generated file
                            fileWriter.WriteToFile(createdFilePath, extractedText);
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