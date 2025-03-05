using System;
using System.Drawing;
using System.Drawing.Imaging; 
using System.IO;
using Tesseract;
using OCRProject.Utils;
using Utils;

namespace OCRProject.TesseractProcessor
{
    public class TesseractProcessor
    {
        public static void ExtractTextFromImage(Bitmap image, string createdFilePath, FileWriter fileWriter)
        {
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
                                string extractedText = page.GetText();
                                Console.WriteLine($"Extracted text: {extractedText}");

                                // Write extracted text to the generated file
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
        }
    }
}
