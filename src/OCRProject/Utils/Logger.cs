using System;
using System.IO;

namespace OCRProject.Utils
{
    public class Logger
    {
        private readonly string _logFilePath;

        public Logger()
        {
            var config = new ConfigLoader();
            string logDirectory = config.LogFolder; // Config should provide a folder, not a file

            // Ensure the log directory exists
            Directory.CreateDirectory(logDirectory);

            // Set the log file inside the directory
            _logFilePath = Path.Combine(logDirectory, "AppLog.txt");
        }

        public void LogError(string message)
        {
            Log("[ERROR]", message);
        }

        public void LogInfo(string message)
        {
            Log("[INFO]", message);
        }

        // Add this method for warnings
        public void LogWarning(string message)
        {
            Log("[WARNING]", message);
        }

        private void Log(string logLevel, string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{logLevel} {DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error: {ex.Message}");
            }
        }
    }
}