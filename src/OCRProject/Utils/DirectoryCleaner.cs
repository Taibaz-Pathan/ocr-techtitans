using System;
using System.IO;
using OCRProject.Utils;

namespace OCRProject.Utils
{
    public class DirectoryCleaner
    {
        private readonly Logger _logger;

        public DirectoryCleaner(Logger logger)
        {
            _logger = logger;
        }

        // Method to clean the contents of the directory
        public void CleanDirectory(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                        _logger.LogInfo($"Deleted file: {file}");
                    }
                }
                else
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInfo($"Directory created: {folderPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cleaning directory {folderPath}: {ex.Message}");
            }
        }
    }
}
