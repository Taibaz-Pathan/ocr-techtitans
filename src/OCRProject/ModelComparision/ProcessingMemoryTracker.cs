using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace OCRProject.ModelComparison
{
    public class ProcessingMemoryTracker
    {
        private readonly List<(string ImageName, string ProcessingStep, double MemoryUsedInMB)> memoryUsages;
        private readonly string outputExcelPath;

        public ProcessingMemoryTracker(string outputFolder)
        {
            memoryUsages = new List<(string, string, double)>();
            outputExcelPath = Path.Combine(outputFolder, "ProcessingResults.xlsx");
        }

        /// <summary>
        /// Measures memory used before and after the given action.
        /// </summary>
        public void MeasureMemoryUsage(string imageName, string processingStep, Action action)
        {
            var process = Process.GetCurrentProcess();
            long memoryBefore = process.PrivateMemorySize64;

            action();

            process.Refresh(); // Refresh the memory info
            long memoryAfter = process.PrivateMemorySize64;

            double memoryUsedInMB = (memoryAfter - memoryBefore) / (1024.0 * 1024.0);
            memoryUsages.Add((imageName, processingStep, Math.Round(memoryUsedInMB, 2)));
        }


        /// <summary>
        /// Appends memory usage data to the same Excel file where time tracking is stored.
        /// </summary>
        public void AppendMemoryUsageToExcel()
        {
            if (!File.Exists(outputExcelPath))
            {
                Console.WriteLine("Excel file not found. Cannot write memory usage.");
                return;
            }

            IWorkbook workbook;

            using (FileStream fileStream = new FileStream(outputExcelPath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }

            ISheet sheet = workbook.GetSheet("Processing Times") ?? workbook.CreateSheet("Processing Times");

            IRow headerRow = sheet.GetRow(0) ?? sheet.CreateRow(0);
            int memoryColumnIndex = headerRow.LastCellNum;

            // Add header if it doesn't already exist
            if (headerRow.Cells.All(c => c.StringCellValue != "Memory Usage (MB)"))
            {
                headerRow.CreateCell(memoryColumnIndex).SetCellValue("Memory Usage (MB)");
            }
            else
            {
                memoryColumnIndex = headerRow.Cells.FindIndex(c => c.StringCellValue == "Memory Usage (MB)");
            }

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow? row = sheet.GetRow(i);
                if (row == null || row.Cells.Count < 2) continue;

                string imageName = row.GetCell(0)?.ToString() ?? string.Empty;
                string step = row.GetCell(1)?.ToString() ?? string.Empty;

                var match = memoryUsages.FirstOrDefault(m => m.ImageName == imageName && m.ProcessingStep == step);

                if (match != default)
                {
                    ICell cell = row.CreateCell(memoryColumnIndex);
                    cell.SetCellValue(match.MemoryUsedInMB);
                }
            }

            // Write to memory first, then to file
            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                workbook.Close();
                File.WriteAllBytes(outputExcelPath, memoryStream.ToArray());
            }
        }
    }
}
