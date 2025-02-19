using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;
using Tesseract; 
using System.Windows.Forms;
using OCRProject.ImageProcessing;
using OCRProject.Utils;
using System.IO.Pipelines;
using OCRProject.TesseractProcessor;
using OCRProject.ModelComparison;

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
            string comparisionResults=config.ComparisionFolder;

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
                    ImageDisplayer.ShowImage(inputImage, "Original Image");

                    ProcessingTimeTracker timeTracker = new ProcessingTimeTracker(comparisionResults);

                    // Convert to grayscale
                    timeTracker.StartTimer();
                    ConvertToGrayscale grayscaleConverter = new ConvertToGrayscale();
                    Bitmap grayImage = grayscaleConverter.Apply(inputImage);
                    //ShowImage(grayImage, "Grayscale Image");
                    grayImage.Save(Path.Combine(outputFolderImage, fileName + "_grayscale.png"), System.Drawing.Imaging.ImageFormat.Png);
                    TesseractProcessor.ExtractTextFromImage(grayImage, createdFilePath, fileWriter);
                    timeTracker.StopAndRecord(fileName, "Grayscale Conversion");

                    // Apply adaptive thresholding
                    timeTracker.StartTimer();
                    AdaptiveThreshold adaptiveThreshold = new AdaptiveThreshold();
                    Bitmap adaptiveThresholdImage = adaptiveThreshold.ApplyThreshold(inputImage);
                    //ShowImage(adaptiveThresholdImage, "Adaptive Threshold Image");
                    adaptiveThresholdImage.Save(Path.Combine(outputFolderImage, fileName + "_AdaptiveThreshold.png"), System.Drawing.Imaging.ImageFormat.Png);
                    TesseractProcessor.ExtractTextFromImage(adaptiveThresholdImage, createdFilePath, fileWriter);
                    timeTracker.StopAndRecord(fileName, "AdaptiveThreshold");

                    // Apply global thresholding (Example: threshold = 128)
                    timeTracker.StartTimer();
                    GlobalThresholding globalThresholding = new GlobalThresholding(128);
                    Bitmap globalThresholdImage = globalThresholding.ApplyThreshold(inputImage);
                    //ShowImage(globalThresholdImage, "Global Threshold Image");
                    globalThresholdImage.Save(Path.Combine(outputFolderImage, fileName + "globalThreshold.png"), System.Drawing.Imaging.ImageFormat.Png);
                    TesseractProcessor.ExtractTextFromImage(globalThresholdImage, createdFilePath, fileWriter);
                    timeTracker.StopAndRecord(fileName, "globalThreshold");

                    timeTracker.StartTimer();
                    ShiftImage shiftProcessor = new ShiftImage();
                    Bitmap shiftedImage = shiftProcessor.Apply(inputImage, 5, 5);
                    //ShowImage(shiftedImage, "Shifted Image");
                    shiftedImage.Save(Path.Combine(outputFolderImage, fileName + "_shiftedImage.png"), System.Drawing.Imaging.ImageFormat.Png);
                    TesseractProcessor.ExtractTextFromImage(shiftedImage, createdFilePath, fileWriter);
                    timeTracker.StopAndRecord(fileName, "ShiftImage");

                    timeTracker.StartTimer();
                    SaturationAdjustment saturationProcessor = new SaturationAdjustment();
                    Bitmap adjustedImage = saturationProcessor.Apply(globalThresholdImage, 1.2f); // Adjust saturation factor
                    //ShowImage(adjustedImage, "Saturation Adjusted Image");
                    shiftedImage.Save(Path.Combine(outputFolderImage, fileName + "_SaturationAjusted.png"), System.Drawing.Imaging.ImageFormat.Png);
                    TesseractProcessor.ExtractTextFromImage(adjustedImage, createdFilePath, fileWriter);
                    timeTracker.StopAndRecord(fileName, "SaturationAdjustment");

                    //deskew processing
                    timeTracker.StartTimer();
                    Deskew deskewProcessor = new Deskew();
                    Bitmap deskewImage = deskewProcessor.Apply(inputImage);
                    ImageDisplayer.ShowImage(deskewImage, "Deskew Image");
                    deskewImage.Save(Path.Combine(outputFolderImage, fileName + "_DeskewImage.png"), System.Drawing.Imaging.ImageFormat.Png);
                    TesseractProcessor.ExtractTextFromImage(deskewImage, createdFilePath, fileWriter);
                    timeTracker.StopAndRecord(fileName, "Deskew");

                    //Generate Report
                    timeTracker.GenerateExcelReport();

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
    
}