using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Utils;

namespace ImageProcessing
{
    public class AdaptiveThresholdProcessor
    {
        public void Process()
        {            
            {
                // Load configuration using ConfigLoader
                ConfigLoader config = new ConfigLoader();
                string inputFolder = config.InputFolder;
                string outputFolder = config.ExtractedTextFolder;

                // Ensure output directory exists
                Directory.CreateDirectory(outputFolder);

                // Process all images in the input folder
                foreach (string inputFilePath in Directory.GetFiles(inputFolder, "*.jpg"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    string outputFilePath = Path.Combine(outputFolder, fileName + ".png");
                    
                }

                Console.WriteLine("All images processed successfully.");
            }
            
        }      
    
    }
}
