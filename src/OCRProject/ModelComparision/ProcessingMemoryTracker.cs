using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OCRProject.ModelComparision;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparison
{
    /// <summary>
    /// Tracks memory usage for different processing steps in OCR image processing.
    /// </summary>
    public class ProcessingMemoryTracker : IProcessingMemoryTracker
    {
        private readonly List<(string ImageName, string ProcessingStep, double MemoryUsedInMB)> memoryUsages;
        private readonly string outputExcelPath;

        /// <summary>
        /// Initializes the memory tracker and sets the output Excel file path.
        /// </summary>
        /// <param name="outputFolder">The folder where the Excel file will be saved.</param>
        public ProcessingMemoryTracker(string outputFolder)
        {
            memoryUsages = new List<(string, string, double)>();
            outputExcelPath = Path.Combine(outputFolder, "ProcessingResults.xlsx");
        }

        /// <summary>
        /// Measures the memory usage of a given processing step.
        /// </summary>
        /// <param name="imageName">The name of the image being processed.</param>
        /// <param name="processingStep">The specific step in the image processing pipeline.</param>
        /// <param name="action">The processing action whose memory usage needs to be measured.</param>
        public void MeasureMemoryUsage(string imageName, string processingStep, Action action)
        {
            var process = Process.GetCurrentProcess();
            long memoryBefore = process.PrivateMemorySize64; // Memory before execution

            action(); // Execute the processing step

            process.Refresh(); // Refresh memory info after execution
            long memoryAfter = process.PrivateMemorySize64; // Memory after execution

            double memoryUsedInMB = (memoryAfter - memoryBefore) / (1024.0 * 1024.0);
            memoryUsages.Add((imageName, processingStep, Math.Round(memoryUsedInMB, 2))); // Store memory usage
        }

        /// <summary>
        /// Appends the recorded memory usage data to an existing Excel file.
        /// </summary>
        public void AppendMemoryUsageToExcel()
        {
            if (!File.Exists(outputExcelPath))
            {
                Console.WriteLine("Excel file not found. Cannot write memory usage.");
                return;
            }

            IWorkbook workbook;

            // Open the existing Excel file
            using (FileStream fileStream = new FileStream(outputExcelPath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }

            // Retrieve or create the "Processing Times" sheet
            ISheet sheet = workbook.GetSheet("Processing Times") ?? workbook.CreateSheet("Processing Times");

            // Get or create the header row
            IRow headerRow = sheet.GetRow(0) ?? sheet.CreateRow(0);
            int memoryColumnIndex = headerRow.LastCellNum; // Identify next available column

            // Add "Memory Usage (MB)" column if it doesn't exist
            if (headerRow.Cells.All(c => c.StringCellValue != "Memory Usage (MB)"))
            {
                headerRow.CreateCell(memoryColumnIndex).SetCellValue("Memory Usage (MB)");
            }
            else
            {
                memoryColumnIndex = headerRow.Cells.FindIndex(c => c.StringCellValue == "Memory Usage (MB)");
            }

            // Update each row with recorded memory usage
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow? row = sheet.GetRow(i);
                if (row == null || row.Cells.Count < 2) continue; // Skip if the row is empty or incomplete

                string imageName = row.GetCell(0)?.ToString() ?? string.Empty;
                string step = row.GetCell(1)?.ToString() ?? string.Empty;

                // Find matching memory usage record
                var match = memoryUsages.FirstOrDefault(m => m.ImageName == imageName && m.ProcessingStep == step);

                if (match != default)
                {
                    ICell cell = row.CreateCell(memoryColumnIndex);
                    cell.SetCellValue(match.MemoryUsedInMB); // Write memory usage
                }
            }

            // Save the updated Excel file
            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                workbook.Close();
                File.WriteAllBytes(outputExcelPath, memoryStream.ToArray());
            }
        }
    }
}