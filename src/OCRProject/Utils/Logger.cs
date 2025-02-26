using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRProject.Utils
{
    public class Logger

    {
        private readonly string _logFilePath;
        public Logger()
        {
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "AppLog.txt");

            // Ensure the log directory exists 
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));

        }
        public void LogError(string message)
        {
            File.AppendAllText(_logFilePath, $"[ERROR] {DateTime.Now}: {message}{Environment.NewLine}");

        }
        public void LogInfo(string message)
        {
            File.AppendAllText(_logFilePath, $"[INFO] {DateTime.Now}: {message}{Environment.NewLine}");

        }

    }
}
