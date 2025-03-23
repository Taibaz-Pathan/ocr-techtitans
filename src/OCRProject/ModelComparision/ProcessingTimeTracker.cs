using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparison
{
    /// <summary>
    /// Tracks and records processing time for different steps in the OCR pipeline.
    /// </summary>
    public class ProcessingTimeTracker : IProcessingTimeTracker
    {
        private Stopwatch stopwatch;
        private List<(string ImageName, string ProcessingStep, double TimeTaken)> processingTimes;
        private string outputExcelPath;

        /// <summary>
        /// Initializes the processing time tracker and sets the output file path.
        /// </summary>
        /// <param name="outputFolder">The directory where the Excel report will be stored.</param>
        public ProcessingTimeTracker(string outputFolder)
        {
            stopwatch = new Stopwatch();
            processingTimes = new List<(string, string, double)>();
            outputExcelPath = Path.Combine(outputFolder, "ProcessingResults.xlsx");
        }

        /// <summary>
        /// Starts the stopwatch for measuring processing time.
        /// </summary>
        public void StartTimer()
        {
            stopwatch.Restart();
        }

        /// <summary>
        /// Stops the timer and records the elapsed time for a specific image and processing step.
        /// </summary>
        /// <param name="imageName">The name of the image being processed.</param>
        /// <param name="processingStep">The specific step in the OCR pipeline.</param>
        public void StopAndRecord(string imageName, string processingStep)
        {
            stopwatch.Stop();
            processingTimes.Add((imageName, processingStep, stopwatch.Elapsed.TotalSeconds));
        }

        /// <summary>
        /// Generates an Excel report containing the recorded processing times.
        /// </summary>
        public void GenerateExcelReport()
        {
            // Create a new workbook and sheet for storing processing times
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Processing Times");

            // Create header row
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Image Name");
            headerRow.CreateCell(1).SetCellValue("Processing Step");
            headerRow.CreateCell(2).SetCellValue("Time Taken (s)");

            // Add recorded data
            int rowIndex = 1;
            foreach (var entry in processingTimes)
            {
                IRow row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(entry.ImageName);
                row.CreateCell(1).SetCellValue(entry.ProcessingStep);
                row.CreateCell(2).SetCellValue(entry.TimeTaken);
            }

            // Save the workbook to the specified file
            using (FileStream fileStream = new FileStream(outputExcelPath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }

            workbook.Close();
        }
    }
}