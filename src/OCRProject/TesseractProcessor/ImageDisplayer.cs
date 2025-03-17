using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace OCRProject.TesseractProcessor
{
    public class ImageDisplayer
    {
        public static async Task ShowImage(Image<Rgba32> image, string title)
        {
            // Save the image as a temporary file
            string tempPath = Path.Combine(Path.GetTempPath(), "temp_display_image.png");
            await image.SaveAsync(tempPath, new PngEncoder());

            Console.WriteLine($"Displaying image: {title}");

            // Open the image in the default viewer
            OpenImage(tempPath);

            // Wait for 3 seconds before automatically closing
            await Task.Delay(3000);
        }

        private static void OpenImage(string filePath)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    System.Diagnostics.Process.Start("explorer", filePath);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    System.Diagnostics.Process.Start("open", filePath);
                }
                else if (OperatingSystem.IsLinux())
                {
                    System.Diagnostics.Process.Start("xdg-open", filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening image: {ex.Message}");
            }
        }
    }
}
