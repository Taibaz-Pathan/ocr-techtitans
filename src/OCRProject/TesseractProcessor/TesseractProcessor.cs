using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using Tesseract;
using OCRProject.Utils;

namespace OCRProject.TesseractProcessor
{
    public class TesseractProcessor
    {
        public static string ExtractTextFromImage(Image<Rgba32> image, string createdFilePath, FileWriter fileWriter)
        {
            string extractedText = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    // Convert ImageSharp Image to PNG stream
                    image.SaveAsPng(stream);
                    stream.Position = 0;

                    // Load the image into Tesseract Pix format
                    using (var pixImage = Pix.LoadFromMemory(stream.ToArray()))
                    {
                        using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                        {
                            using (var page = engine.Process(pixImage))
                            {
                                extractedText = page.GetText();  // Extract text
                                fileWriter.WriteToFile(createdFilePath, extractedText); // Save text
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExtractTextFromImage: {ex.Message}");
            }

            return extractedText; // Return extracted text
        }
    }
}
