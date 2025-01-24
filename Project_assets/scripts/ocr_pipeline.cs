using System;
using OpenCvSharp;
using Tesseract;

public class OCRPipeline
{
    public static void RunOCRPipeline(string inputImagePath, string outputImagePath)
    {
        // Step 1: Preprocess the image
        ConvertToGrayscale(inputImagePath, outputImagePath);

        // Step 2: Perform OCR
        PerformOCR(outputImagePath);
    }

    private static void ConvertToGrayscale(string inputPath, string outputPath)
    {
        Mat image = Cv2.ImRead(inputPath);
        Mat grayImage = new Mat();
        Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
        Cv2.ImWrite(outputPath, grayImage);
    }

    private static void PerformOCR(string inputImagePath)
    {
        try
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(inputImagePath))
                {
                    var result = engine.Process(img);
                    Console.WriteLine("OCR Result: " + result.GetText());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during OCR: " + ex.Message);
        }
    }
}