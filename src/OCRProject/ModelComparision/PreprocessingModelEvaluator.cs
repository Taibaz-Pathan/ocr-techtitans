using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparision
{
    /// <summary>
    /// Evaluates preprocessing models based on cosine similarity and performance metrics.
    /// </summary>
    public class PreprocessingModelEvaluator : IPreprocessingModelEvaluator
    {
        private readonly string _similarityFilePath;
        private readonly string _performanceFilePath;

        /// <summary>
        /// Initializes the evaluator with file paths for similarity and performance data.
        /// </summary>
        /// <param name="similarityFilePath">Path to the Excel file containing cosine similarity scores.</param>
        /// <param name="performanceFilePath">Path to the Excel file containing performance metrics.</param>
        public PreprocessingModelEvaluator(string similarityFilePath, string performanceFilePath)
        {
            _similarityFilePath = similarityFilePath;
            _performanceFilePath = performanceFilePath;
        }

        /// <summary>
        /// Evaluates models based on similarity and performance, ranks them, and generates a report.
        /// </summary>
        public void EvaluateAndReportBestModels()
        {
            var similarityScores = LoadCosineSimilarity();
            var performanceMetrics = LoadPerformanceData();

            var models = similarityScores.Keys.Intersect(performanceMetrics.Keys).ToList();
            var report = new List<(string Model, double FinalScore)>();

            // Normalize values for fair comparison
            double maxSimilarity = similarityScores.Values.Max();
            double maxTime = performanceMetrics.Values.Max(v => v.TimeTaken);
            double maxMemory = performanceMetrics.Values.Max(v => v.MemoryUsage);

            foreach (var model in models)
            {
                double normalizedSim = similarityScores[model] / maxSimilarity;
                double normalizedTime = 1 - (performanceMetrics[model].TimeTaken / maxTime); // Lower is better
                double normalizedMem = 1 - (performanceMetrics[model].MemoryUsage / maxMemory); // Lower is better

                // Weighted scoring
                double finalScore = (0.5 * normalizedSim) + (0.3 * normalizedTime) + (0.2 * normalizedMem);
                report.Add((model, finalScore));
            }

            var sortedReport = report.OrderByDescending(r => r.FinalScore).ToList();

            // Display the ranking in console
            Console.WriteLine("\n📊 Preprocessing Model Evaluation Report (Best to Worst):\n");
            int rank = 1;
            foreach (var item in sortedReport)
            {
                Console.WriteLine($"{rank++}. {item.Model} - Final Score: {item.FinalScore:F4}");
            }

            SaveReportToExcel(sortedReport);
        }

        /// <summary>
        /// Loads cosine similarity scores from an Excel file.
        /// </summary>
        /// <returns>A dictionary with model names as keys and their cosine similarity scores as values.</returns>
        private Dictionary<string, double> LoadCosineSimilarity()
        {
            var result = new Dictionary<string, double>();

            using (var fileStream = new FileStream(_similarityFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);

                for (int row = 1; row <= sheet.LastRowNum; row++) // Skip header
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    string model = currentRow.GetCell(0)?.ToString() ?? "";
                    double score = double.TryParse(currentRow.GetCell(1)?.ToString(), out var val) ? val : 0;
                    result[model] = score;
                }
            }

            return result;
        }

        /// <summary>
        /// Loads performance metrics from an Excel file.
        /// </summary>
        /// <returns>A dictionary with model names as keys and a tuple containing time taken and memory usage.</returns>
        private Dictionary<string, (double TimeTaken, double MemoryUsage)> LoadPerformanceData()
        {
            var result = new Dictionary<string, (double, double)>();

            using (var fileStream = new FileStream(_performanceFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);

                for (int row = 1; row <= sheet.LastRowNum; row++) // Skip header
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    string model = currentRow.GetCell(1)?.ToString() ?? "";
                    double time = double.TryParse(currentRow.GetCell(2)?.ToString(), out var t) ? t : 0;
                    double mem = double.TryParse(currentRow.GetCell(3)?.ToString(), out var m) ? m : 0;
                    result[model] = (time, mem);
                }
            }

            return result;
        }

        /// <summary>
        /// Saves the ranked model evaluation report to an Excel file.
        /// </summary>
        /// <param name="report">A list of tuples containing model names and their final scores.</param>
        private void SaveReportToExcel(List<(string Model, double FinalScore)> report)
        {
            string outputFolder = Path.GetDirectoryName(_similarityFilePath) ?? Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(outputFolder, "BestModelRanking.xlsx");

            using (var workbook = new XSSFWorkbook())
            {
                ISheet sheet = workbook.CreateSheet("Model Ranking");

                // Create header row
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Rank");
                headerRow.CreateCell(1).SetCellValue("Model");
                headerRow.CreateCell(2).SetCellValue("Final Score");

                // Fill data
                for (int i = 0; i < report.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(i + 1);
                    row.CreateCell(1).SetCellValue(report[i].Model);
                    row.CreateCell(2).SetCellValue(report[i].FinalScore);
                }

                // Save the Excel file
                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                Console.WriteLine($"Report saved successfully to: {outputPath}");
            }
        }
    }
}