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
                // Debug output (currently commented out)
                //Console.WriteLine($"WriteToFile - filePath: {filePath}");
                //Console.WriteLine($"WriteToFile - text: {text}");

                // Check if either the file path or the text is null or empty
                if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(text))
                {
                    Console.WriteLine("Invalid file path or text."); // Error message for invalid inputs
                    return;
                }

                // Check if the provided file path is absolute
                if (!Path.IsPathRooted(filePath))
                {
                    Console.WriteLine("File path is not absolute."); // Error message for relative paths
                    return;
                }

                // Ensure the file exists before trying to write to it
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"The file does not exist at {filePath}. Ensure the file creation step has been successful."); // Inform user that the file doesn't exist
                    return;
                }

                // Append the provided text to the file, adding a new line after the text
                File.AppendAllText(filePath, text + Environment.NewLine);
                //Console.WriteLine("Text written to file successfully."); // Success message (currently commented out)
            }
            catch (Exception ex)
            {
                // Catch any exceptions and display the error message
                Console.WriteLine($"Error writing to file: {ex.Message}"); // Display error details if something goes wrong
            }
        }
    }
}
