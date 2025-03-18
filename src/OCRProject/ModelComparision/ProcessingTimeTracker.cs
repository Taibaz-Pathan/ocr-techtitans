using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparison
{
    public class ProcessingTimeTracker : IProcessingTimeTracker
    {
        private Stopwatch stopwatch;
        private List<(string ImageName, string ProcessingStep, double TimeTaken)> processingTimes;
        private string outputExcelPath;

        public ProcessingTimeTracker(string outputFolder)
        {
            stopwatch = new Stopwatch();
            processingTimes = new List<(string, string, double)>();
            outputExcelPath = Path.Combine(outputFolder, "ProcessingResults.xlsx");
        }

        public void StartTimer()
        {
            stopwatch.Restart();
        }

        public void StopAndRecord(string imageName, string processingStep)
        {
            stopwatch.Stop();
            processingTimes.Add((imageName, processingStep, stopwatch.Elapsed.TotalSeconds));
        }

        public void GenerateExcelReport()
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Processing Times");

            // Create Header Row
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Image Name");
            headerRow.CreateCell(1).SetCellValue("Processing Step");
            headerRow.CreateCell(2).SetCellValue("Time Taken (s)");

            // Add Data Rows
            int rowIndex = 1;
            foreach (var entry in processingTimes)
            {
                IRow row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(entry.ImageName);
                row.CreateCell(1).SetCellValue(entry.ProcessingStep);
                row.CreateCell(2).SetCellValue(entry.TimeTaken);
            }

            // Save to file
            using (FileStream fileStream = new FileStream(outputExcelPath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }
            
            workbook.Close();
        }
    }
}
