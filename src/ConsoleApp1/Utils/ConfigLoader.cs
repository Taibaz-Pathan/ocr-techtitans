using System;
using System.IO;
using Utils;
using System.Reflection;

namespace Utils
{
    public class ConfigLoader
    {        

        public string InputFolder { get; private set; }       
        public string ExtractedTextFolder { get; private set; }
        

        public ConfigLoader()
        {

            // Get the current working directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Define the relative path to the input folder
            string relativePath = @"../../../../../Input/SampleImages";
            
            // Combine the current directory and the relative path
            string InputFolderpath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));
            InputFolder = Path.GetFullPath(InputFolderpath);

            ExtractedTextFolder = Path.Combine(Directory.GetCurrentDirectory(), "Output", "ExtractedText");
                
            // Ensure directories exist
            EnsureDirectoriesExist();
            
        }

        private void EnsureDirectoriesExist()
        {
            var directories = new List<string>
            {
                InputFolder,
                ExtractedTextFolder                
            };

            
        }
    }
}
