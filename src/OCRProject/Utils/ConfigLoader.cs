using System;
using System.IO;
using Utils;
using System.Reflection;

namespace Utils
{
    public class ConfigLoader
    {
        public string InputFolder { get; private set; }
        public string OutputImageFolder { get; private set; }
        public string ExtractedTextFolder { get; private set; }
        public string ComparisionFolder { get; private set; }

        public ConfigLoader()
        {
            // Get the current working directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Define the relative path to the input and Output folder
            string relativePath = @"../../../../OCRProject";           

            // Combine the current directory and the relative path for Input
            string InputFolderpath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Input"));
            InputFolder = Path.GetFullPath(InputFolderpath);

            // Combine the current directory and the relative path for Processed Image
            string OuputFolderpath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "ProcessedImage"));
            OutputImageFolder= Path.GetFullPath(OuputFolderpath);

            //Path to save extracted text
            string extractedtext = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "ExtractedText"));
            ExtractedTextFolder = Path.GetFullPath(extractedtext);
            
            //Path to store comparision Results
            string comparision = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "Comparision"));
            ComparisionFolder = Path.GetFullPath(comparision);

            // Ensure directories exist
            EnsureDirectoriesExist();

        }
        private void EnsureDirectoriesExist()
        {
            var directories = new List<string>
            {
                InputFolder,
                OutputImageFolder,
                ExtractedTextFolder
            };

        }
    }
}
