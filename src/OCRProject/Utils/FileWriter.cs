using System;
using System.IO;

namespace OCRProject.Utils
{
    public class FileWriter
    {
        /// <summary>
        /// Writes text into the specified file.
        /// </summary>
        /// <param name="filePath">The full path of the file.</param>
        /// <param name="text">The text to write into the file.</param>
        public void WriteToFile(string filePath, string text)
        {
            try
            {
                // Debug output
                //Console.WriteLine($"WriteToFile - filePath: {filePath}");
                //Console.WriteLine($"WriteToFile - text: {text}");

                if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(text))
                {
                    Console.WriteLine("Invalid file path or text.");
                    return;
                }

                // Check if the file path is valid
                if (!Path.IsPathRooted(filePath))
                {
                    Console.WriteLine("File path is not absolute.");
                    return;
                }

                // Ensure the file exists and is writable
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"The file does not exist at {filePath}. Ensure the file creation step has been successful.");
                    return;
                }

                // Append text to file
                File.AppendAllText(filePath, text + Environment.NewLine);
                //Console.WriteLine("Text written to file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
    }
}