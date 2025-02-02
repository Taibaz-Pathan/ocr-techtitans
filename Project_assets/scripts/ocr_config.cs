using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Tesseract;
using System.Drawing;
using System.Drawing.Imaging;

class OCRProcessor
{
    // Define OCR Config structure
    class OCRConfig
    {
        public List<string> language { get; set; }
        public PreprocessingConfig preprocessing { get; set; }
        public ModelConfig model { get; set; }
        public OutputConfig output { get; set; }
    }

    class PreprocessingConfig
    {
        public bool grayscale { get; set; }
        public bool thresholding { get; set; }
        public int threshold_value { get; set; }
        public bool denoise { get; set; }
        public int denoise_strength { get; set; }
        public bool contrast_enhancement { get; set; }
        public bool edge_detection { get; set; }
    }

    class ModelConfig
    {
        public string path { get; set; }
        public string type { get; set; }
    }

    class OutputConfig
    {
        public List<string> format { get; set; }
        public string save_path { get; set; }
        public bool backup { get; set; }
    }

    static void Main()
    {
        // Load configuration
        string configPath = "ocr_config.json";
        OCRConfig config = JsonConvert.DeserializeObject<OCRConfig>(File.ReadAllText(configPath));

        // Define image path
        string imagePath = "input/sample_image.png";

        // Preprocess and extract text
        string extractedText = PerformOCR(imagePath, config);

        // Save output
        SaveResults(extractedText, config.output);
    }

    static string PerformOCR(string imagePath, OCRConfig config)
    {
        using (var engine = new TesseractEngine(config.model.path, string.Join("+", config.language), EngineMode.Default))
        {
            // Load and preprocess image
            Bitmap processedImage = PreprocessImage(imagePath, config.preprocessing);

            // Perform OCR
            using (var page = engine.Process(processedImage))
            {
                return page.GetText();
            }
        }
    }

    static Bitmap PreprocessImage(string imagePath, PreprocessingConfig preprocessing)
    {
        Bitmap image = new Bitmap(imagePath);

        if (preprocessing.grayscale)
        {
            image = ConvertToGrayscale(image);
        }

        if (preprocessing.thresholding)
        {
            image = ApplyThresholding(image, preprocessing.threshold_value);
        }

        return image;
    }

    static Bitmap ConvertToGrayscale(Bitmap image)
    {
        Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);
        using (Graphics g = Graphics.FromImage(grayscaleImage))
        {
            var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                new float[][] {
                    new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                    new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                    new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

            var attributes = new System.Drawing.Imaging.ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }
        return grayscaleImage;
    }

    static Bitmap ApplyThresholding(Bitmap image, int threshold)
    {
        Bitmap thresholdedImage = new Bitmap(image.Width, image.Height);
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Color pixelColor = image.GetPixel(x, y);
                int brightness = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                Color newColor = (brightness < threshold) ? Color.Black : Color.White;
                thresholdedImage.SetPixel(x, y, newColor);
            }
        }
        return thresholdedImage;
    }

    static void SaveResults(string text, OutputConfig outputConfig)
    {
        string savePath = outputConfig.save_path;

        if (outputConfig.format.Contains("json"))
        {
            File.WriteAllText($"{savePath}.json", JsonConvert.SerializeObject(new { text }));
        }

        if (outputConfig.format.Contains("txt"))
        {
            File.WriteAllText($"{savePath}.txt", text);
        }

        if (outputConfig.format.Contains("csv"))
        {
            File.WriteAllText($"{savePath}.csv", "Extracted Text\n" + text.Replace("\n", " "));
        }
    }
}