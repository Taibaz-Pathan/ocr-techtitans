using System;
using System.IO;

namespace OCRProject.Utils
{
    public class CreateFiles
    {
        /// <summary>
        /// Creates an empty text file in the specified folder.
        /// </summary>
        /// <param name="folderPath">The directory where the file should be created.</param>
        /// <param name="fileName">The name of the file to create.</param>
        /// <returns>The full path of the created file, or an empty string if creation fails.</returns>
        public string CreateTextFile(string folderPath, string fileName)
        {
            try
            {
                // Ensure the directory exists, if not, create it
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);  // Create the directory if it doesn't exist
                }

                // Combine the folder path and file name to get the full file path
                string filePath = Path.Combine(folderPath, fileName);

                // Check if the file already exists
                if (!File.Exists(filePath))
                {
                    // Create an empty file if it doesn't exist
                    using (File.Create(filePath)) { }  // Create the file and close it immediately
                    Console.WriteLine($"File created: {filePath}");  // Log that the file was created
                }
                else
                {
                    // Log that the file already exists
                    Console.WriteLine($"File already exists: {filePath}");
                }

                // Return the full path of the created (or existing) file
                return filePath;
            }
            catch (Exception ex)
            {
                // Catch any errors (e.g., permissions issue) and log the exception message
                Console.WriteLine($"Error creating file: {ex.Message}");

                // Return an empty string to indicate that file creation failed
                return string.Empty;  // Avoid returning null, return an empty string instead
            }
        }
    }
}
