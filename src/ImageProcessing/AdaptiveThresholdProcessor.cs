﻿using System;
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

                    // Display and process the image
                    DisplayImage(inputFilePath);

                }
            }
        }

        private void DisplayImage(string imagePath)
        {
            using (Bitmap image = new Bitmap(imagePath))
                {
                    Console.WriteLine($"Displaying image: {imagePath}");
                    using (var form = new System.Windows.Forms.Form())
                    {
                        form.Text = "Input Image";
                        form.ClientSize = new Size(image.Width, image.Height);

                        var pictureBox = new System.Windows.Forms.PictureBox
                        {
                            Dock = System.Windows.Forms.DockStyle.Fill,
                            Image = image,
                            SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
                        };

                        form.Controls.Add(pictureBox);
                        System.Windows.Forms.Application.Run(form);
                    }
                }
        }
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Adaptive Threshold Processor...");

                // Create an instance of the processor
                AdaptiveThresholdProcessor processor = new AdaptiveThresholdProcessor();

                // Run the processing task
                processor.ProcessImages();

                Console.WriteLine("Processing completed. Press any key to exit.");
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                Console.ReadKey();
            }
        }

    }
}