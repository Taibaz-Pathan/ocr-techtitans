using OpenCvSharp;

public class ImagePreprocessing
{
    public static void SimpleThresholding(string inputPath, string outputPath)
    {
        // Load the image in grayscale
        Mat image = Cv2.ImRead(inputPath, ImreadModes.Grayscale);

        // Apply simple thresholding (binary threshold)
        Mat thresholdedImage = new Mat();
        Cv2.Threshold(image, thresholdedImage, 128, 255, ThresholdTypes.Binary);

        // Save the thresholded image
        Cv2.ImWrite(outputPath, thresholdedImage);
    }
}