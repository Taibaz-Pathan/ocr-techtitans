using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OCRProject.Utils
{
    // This class loads configuration paths for various folders and files used in the project.
    public class ConfigLoader
    {
        // Properties to store paths for different folders and files.
        public string InputFolder { get; private set; }
        public string OutputImageFolder { get; private set; }
        public string ExtractedTextFolder { get; private set; }
        public string ComparisionFolder { get; private set; }
        public string LogFolder { get; private set; }
        public string appkeypath { get; private set; }

        // Constructor to load configuration paths
        public ConfigLoader()
        {
            // Get the current working directory of the application
            string currentDirectory = Directory.GetCurrentDirectory();
            
            // Define the relative path to the OCRProject directory (this is based on your folder structure)
            string relativePath = @"../../../../OCRProject";

            // Combine the current directory and relative path to get the full path to the Input folder
            string InputFolderpath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Input"));
            InputFolder = Path.GetFullPath(InputFolderpath);  // Set the InputFolder property

            // Combine the current directory and relative path to get the full path to the Processed Image folder
            string OuputFolderpath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "ProcessedImage"));
            OutputImageFolder = Path.GetFullPath(OuputFolderpath);  // Set the OutputImageFolder property

            // Path to save extracted text (Output/ExtractedText)
            string extractedtext = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "ExtractedText"));
            ExtractedTextFolder = Path.GetFullPath(extractedtext);  // Set the ExtractedTextFolder property

            // Path to store comparison results (Output/Comparision)
            string comparision = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "Comparision"));
            ComparisionFolder = Path.GetFullPath(comparision);  // Set the ComparisionFolder property

            // Path for logging details (Output/Logs)
            string log = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Output", "Logs"));
            LogFolder = Path.GetFullPath(log);  // Set the LogFolder property

            // Path to application key details (Utils/app.json)
            string appkey = Path.GetFullPath(Path.Combine(currentDirectory, relativePath, "Utils", "mycode.json"));
            appkeypath = Path.GetFullPath(appkey);  // Set the appkeypath property

            // Ensure all necessary directories exist (created if they don't exist)
            EnsureDirectoriesExist();
        }

        // This method checks if all required directories exist, and creates them if they don't
        private void EnsureDirectoriesExist()
        {
            // List of directories to check
            var directories = new List<string>
            {
                InputFolder,  // Input folder
                OutputImageFolder,  // Processed image folder
                ExtractedTextFolder  // Extracted text folder
            };

            // Loop through each directory and create it if it doesn't exist
            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))  // Check if the directory does not exist
                {
                    Directory.CreateDirectory(directory);  // Create the directory
                }
            }
        }
    }
}
