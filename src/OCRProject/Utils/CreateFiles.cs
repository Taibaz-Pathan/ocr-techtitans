using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRProject.Utils
{
    public class CreateFiles
    {
        /// <summary>
        /// Creates an empty text file in the specified folder.
        /// </summary>
        /// <param name="folderPath">The directory where the file should be created.</param>
        /// <param name="fileName">The name of the file to create.</param>
        /// <returns>The full path of the created file.</returns>
        public string CreateTextFile(string folderPath, string fileName)
        {
            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                // Create an empty file if it does not exist
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }
                    //Console.WriteLine($"File created: {filePath}");
                }
                else
                {
                    Console.WriteLine($"File already exists: {filePath}");
                }

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file: {ex.Message}");
                return null;
            }
        }
    }
}
