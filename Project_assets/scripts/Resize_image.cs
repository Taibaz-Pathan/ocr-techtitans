using OpenCvSharp;

public static void ResizeImage(string inputPath, string outputPath, int width, int height)
{
    Mat image = Cv2.ImRead(inputPath, ImreadModes.Color);
    Mat resizedImage = new Mat();
    Cv2.Resize(image, resizedImage, new Size(width, height));
    Cv2.ImWrite(outputPath, resizedImage);
}

// Example
ResizeImage("input_image.jpg", "output_resized.jpg", 1024, 768);