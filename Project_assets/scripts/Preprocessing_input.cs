using System;
using System.IO;
using Newtonsoft.Json;
using OpenCvSharp;

public class Preprocessing
{
    public static void GrayscaleConversion(string jsonPath)
    {
        var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(jsonPath));
        var image = Cv2.ImRead((string)config.input_path);
        var grayImage = new Mat();
        Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
        Cv2.ImWrite((string)config.output_path, grayImage);
    }

    public static void AdaptiveThreshold(string jsonPath)
    {
        var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(jsonPath));
        var image = Cv2.ImRead((string)config.input_path, ImreadModes.Grayscale);
        var thresholdedImage = new Mat();
        Cv2.AdaptiveThreshold(image, thresholdedImage, 255,
            AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 
            (int)config.block_size, (int)config.C);
        Cv2.ImWrite((string)config.output_path, thresholdedImage);
    }

    public static void DenoiseImage(string jsonPath)
    {
        var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(jsonPath));
        var image = Cv2.ImRead((string)config.input_path);
        var denoisedImage = new Mat();
        if ((string)config.method == "gaussian")
        {
            Cv2.GaussianBlur(image, denoisedImage, new Size((int)config.kernel_size, (int)config.kernel_size), 0);
        }
        else if ((string)config.method == "median")
        {
            Cv2.MedianBlur(image, denoisedImage, (int)config.kernel_size);
        }
        Cv2.ImWrite((string)config.output_path, denoisedImage);
    }

    public static void MorphologicalOperation(string jsonPath)
    {
        var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(jsonPath));
        var image = Cv2.ImRead((string)config.input_path, ImreadModes.Grayscale);
        var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size((int)config.kernel_size[0], (int)config.kernel_size[1]));
        var processedImage = new Mat();
        if ((string)config.operation == "closing")
        {
            Cv2.MorphologyEx(image, processedImage, MorphTypes.Close, kernel, iterations: (int)config.iterations);
        }
        else if ((string)config.operation == "opening")
        {
            Cv2.MorphologyEx(image, processedImage, MorphTypes.Open, kernel, iterations: (int)config.iterations);
        }
        Cv2.ImWrite((string)config.output_path, processedImage);
    }
}