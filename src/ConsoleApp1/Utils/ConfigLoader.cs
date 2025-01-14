using System;
using System.IO;
using Utils;

namespace Utils
{
    public class ConfigLoader
    {        

        public string InputFolder { get; private set; }       
        public string ExtractedTextFolder { get; private set; }
        

        public ConfigLoader()
        {

            // Set up paths
            //InputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Input", "SampleImages");
            InputFolder = "C:/Users/mithi/OneDrive/Desktop/SoftwareEngg/ocr-techtitans/Input/SampleImages";         
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
