using System;
using System.IO;
using OCRProject.Utils;

namespace OCRProject.Utils
{
    // This class is responsible for cleaning (deleting) the contents of a specified directory.
    public class DirectoryCleaner
    {
        // Logger instance for logging actions (e.g., deleted files, errors)
        private readonly Logger _logger;

        // Constructor to initialize the DirectoryCleaner with a Logger instance
        public DirectoryCleaner(Logger logger)
        {
            _logger = logger;
        }

        // Method to clean the contents of the specified directory
        public void CleanDirectory(string folderPath)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(folderPath))
                {
                    // Get all files in the directory
                    var files = Directory.GetFiles(folderPath);

                    // Loop through each file and delete it
                    foreach (var file in files)
                    {
                        File.Delete(file); // Delete the file
                        _logger.LogInfo($"Deleted file: {file}"); // Log the deletion
                    }
                }
                else
                {
                    // If the directory does not exist, create it
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInfo($"Directory created: {folderPath}"); // Log the directory creation
                }
            }
            catch (Exception ex)
            {
                // If there is an error (e.g., permission issue, file in use), log the error
                _logger.LogError($"Error cleaning directory {folderPath}: {ex.Message}");
            }
        }
    }
}
