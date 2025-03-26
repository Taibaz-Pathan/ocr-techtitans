using System;
using System.IO;

namespace OCRProject.Utils
{
    // This class is used for logging different levels of messages (Error, Info, Warning) to a log file
    public class Logger
    {
        // The path to the log file where messages will be written
        private readonly string _logFilePath;

        // Constructor to initialize the Logger and determine where to save the log file
        public Logger()
        {
            // Create a new ConfigLoader instance to get the log folder path
            var config = new ConfigLoader();
            string logDirectory = config.LogFolder; // Config should provide a folder, not a file path

            // Ensure that the log directory exists. If it doesn't, create it.
            Directory.CreateDirectory(logDirectory);

            // Set the log file name and path inside the specified log directory
            _logFilePath = Path.Combine(logDirectory, "AppLog.txt");
        }

        // Method to log error messages
        public void LogError(string message)
        {
            // Prepend the log level "[ERROR]" to the message and call the private Log method
            Log("[ERROR]", message);
        }

        // Method to log informational messages
        public void LogInfo(string message)
        {
            // Prepend the log level "[INFO]" to the message and call the private Log method
            Log("[INFO]", message);
        }

        // Method to log warning messages
        public void LogWarning(string message)
        {
            // Prepend the log level "[WARNING]" to the message and call the private Log method
            Log("[WARNING]", message);
        }

        // This private method writes the log message to the log file with the specified log level
        private void Log(string logLevel, string message)
        {
            try
            {
                // Format the log message and append it to the log file
                File.AppendAllText(_logFilePath, $"{logLevel} {DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle permission errors if the log file or directory is not accessible
                Console.WriteLine($"Permission error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other potential errors (e.g., disk full, file system errors)
                Console.WriteLine($"An error occurred while writing to the log file: {ex.Message}");
            }
        }
    }
}
