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
                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);  // Create the directory if it doesn't exist
                }

                string filePath = Path.Combine(folderPath, fileName);

                // Create an empty file if it does not exist
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }  // Create the file and close it
                    Console.WriteLine($"File created: {filePath}");
                }
                else
                {
                    Console.WriteLine($"File already exists: {filePath}");
                }

                return filePath;  // Return the full path of the created file
            }
            catch (Exception ex)
            {
                // Log the error and return an empty string in case of failure
                Console.WriteLine($"Error creating file: {ex.Message}");
                return string.Empty;  // Return an empty string to avoid returning null
            }
        }
    }
}
