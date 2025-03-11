using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace OCRProject.TesseractProcessor
{
    public class EasyOCRProcessor
    {
        public static string ExtractTextFromImage(string imagePath)
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TesseractProcessor", "EasyOCRProcessor.py");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python", // Runs Python without specifying full path
                Arguments = $"\"{scriptPath}\" \"{imagePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        JObject json = JObject.Parse(result);
                        return json["text"]?.ToString() ?? "No text detected";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error in ExtractTextFromImage: {ex.Message}";
            }
        }
    }
}
