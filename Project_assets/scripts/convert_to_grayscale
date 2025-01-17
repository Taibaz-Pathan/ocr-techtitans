using OpenCvSharp;

public static void ConvertToGrayscale(string inputPath, string outputPath)
{
    Mat image = Cv2.ImRead(inputPath, ImreadModes.Color);
    Mat grayImage = new Mat();
    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
    Cv2.ImWrite(outputPath, grayImage);
}

// Example
ConvertToGrayscale("input_image.jpg", "output_grayscale.jpg");