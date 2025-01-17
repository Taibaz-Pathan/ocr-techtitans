using OpenCvSharp;

public static void RemoveNoise(string inputPath, string outputPath)
{
    Mat image = Cv2.ImRead(inputPath, ImreadModes.Grayscale);
    Mat denoisedImage = new Mat();
    Cv2.GaussianBlur(image, denoisedImage, new Size(5, 5), 0);
    Cv2.ImWrite(outputPath, denoisedImage);
}

// Example
RemoveNoise("input_image.jpg", "output_denoised.jpg");