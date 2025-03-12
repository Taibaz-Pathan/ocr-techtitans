using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tesseract;
using OCRProject.Utils;


namespace OCRProject.TesseractProcessor
{
    public class TesseractProcessor
    {
        public static string ExtractTextFromImage(Bitmap image, string createdFilePath, FileWriter fileWriter)
        {
            string extractedText = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    // Save Bitmap as a TIFF in memory
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    stream.Position = 0;

                    // Load the Pix from the memory stream
                    using (var pixImage = Pix.LoadTiffFromMemory(stream.ToArray()))
                    {
                        using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                        {
                            using (var page = engine.Process(pixImage))
                            {
                                extractedText = page.GetText();  // Extract text from image
                                //Console.WriteLine($"Extracted text: {extractedText}");

                                // Write the extracted text to the file using the fileWriter
                                fileWriter.WriteToFile(createdFilePath, extractedText);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExtractTextFromImage: {ex.Message}");
            }

            return extractedText;  // Return the extracted text
        }
    }
}
