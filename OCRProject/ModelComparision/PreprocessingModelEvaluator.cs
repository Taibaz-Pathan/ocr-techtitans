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
    /// This class compares models and generates a report on which models perform best.
    /// </summary>
    public class PreprocessingModelEvaluator : IPreprocessingModelEvaluator
    {
        private readonly string _similarityFilePath;  // Path to the file containing cosine similarity data
        private readonly string _performanceFilePath; // Path to the file containing performance metrics (time and memory usage)

        /// <summary>
        /// Initializes the evaluator with file paths for similarity and performance data.
        /// </summary>
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
            Console.WriteLine("Starting evaluation of best preprocessing models...");

            // Load the cosine similarity scores and performance data from Excel files
            var similarityScores = LoadCosineSimilarity();
            var performanceMetrics = LoadPerformanceData();

            // Debugging: Display the loaded models from both similarity and performance data
            Console.WriteLine("\nLoaded models from Cosine Similarity:");
            foreach (var model in similarityScores.Keys) Console.WriteLine($" - {model}");

            Console.WriteLine("\nLoaded models from Performance Metrics:");
            foreach (var model in performanceMetrics.Keys) Console.WriteLine($" - {model}");

            // Find matching models between similarity and performance metrics (case-insensitive match)
            var models = performanceMetrics.Keys
                .Where(pModel => similarityScores.Keys
                    .Any(sModel => sModel.Equals(pModel, StringComparison.OrdinalIgnoreCase) ||
                                   sModel.Contains(pModel, StringComparison.OrdinalIgnoreCase) ||
                                   pModel.Contains(sModel, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // If no matching models found, show a warning and return
            if (models.Count == 0)
            {
                Console.WriteLine("Warning: No matching models found in both similarity and performance data.");
                return;
            }

            var report = new List<(string Model, double FinalScore)>();  // List to store evaluation report with model and score

            // Calculate the maximum values for similarity, time, and memory to normalize the scores
            double maxSimilarity = similarityScores.Values.DefaultIfEmpty(1.0).Max(); // Default max similarity is 1 if no values
            double maxTime = performanceMetrics.Values.Select(v => v.TimeTaken).DefaultIfEmpty(1.0).Max(); // Default max time is 1
            double maxMemory = performanceMetrics.Values.Select(v => v.MemoryUsage).DefaultIfEmpty(1.0).Max(); // Default max memory is 1

            // Iterate through each matching model to calculate normalized scores and final weighted score
            foreach (var model in models)
            {
                // Retrieve scores for each model, defaulting to max values if not found
                double similarityScore = similarityScores.ContainsKey(model) ? similarityScores[model] : 0;
                double timeTaken = performanceMetrics.ContainsKey(model) ? performanceMetrics[model].TimeTaken : maxTime;
                double memoryUsage = performanceMetrics.ContainsKey(model) ? performanceMetrics[model].MemoryUsage : maxMemory;

                // Normalize the values (inverted for time & memory since lower values are better)
                double normalizedSim = maxSimilarity != 0 ? similarityScore / maxSimilarity : 0;
                double normalizedTime = maxTime != 0 ? 1 - (timeTaken / maxTime) : 0;
                double normalizedMem = maxMemory != 0 ? 1 - (memoryUsage / maxMemory) : 0;

                // Weighted scoring formula
                double finalScore = (0.5 * normalizedSim) + (0.3 * normalizedTime) + (0.2 * normalizedMem);
                report.Add((model, finalScore));  // Add the model and its final score to the report
            }

            // Sort models based on final score, from best to worst
            var sortedReport = report.OrderByDescending(r => r.FinalScore).ToList();

            // Display the sorted models and their scores in the console
            Console.WriteLine("\nPreprocessing Model Evaluation Report (Best to Worst):\n");
            int rank = 1;
            foreach (var item in sortedReport)
            {
                Console.WriteLine($"{rank++}. {item.Model} - Final Score: {item.FinalScore:F4}");
            }

            // Save the ranked models to an Excel file
            SaveReportToExcel(sortedReport);
        }

        /// <summary>
        /// Loads cosine similarity scores from an Excel file.
        /// </summary>
        private Dictionary<string, double> LoadCosineSimilarity()
        {
            var result = new Dictionary<string, double>();  // Dictionary to hold models and their similarity scores

            // Open the Excel file to read similarity scores
            using (var fileStream = new FileStream(_similarityFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);  // Open Excel workbook
                ISheet sheet = workbook.GetSheetAt(0);  // Get the first sheet

                for (int row = 1; row <= sheet.LastRowNum; row++) // Skip the header row
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    string model = currentRow?.GetCell(0)?.ToString()?.Trim() ?? "";  // Get model name from the first cell
                    ICell? cell = currentRow?.GetCell(1);  // Get similarity score from the second cell

                    // Parse the similarity score, defaulting to 0 if invalid
                    double score = 0;
                    if (cell != null && double.TryParse(cell.ToString(), out var val))
                    {
                        score = val;  // If parsing is successful, store the score
                    }

                    // Add to the result dictionary if model is not empty
                    if (!string.IsNullOrEmpty(model))
                    {
                        result[model] = score;  // Store model and score
                    }
                }
            }

            return result;  // Return the loaded similarity scores
        }

        /// <summary>
        /// Loads performance metrics (time and memory usage) from an Excel file.
        /// </summary>
        private Dictionary<string, (double TimeTaken, double MemoryUsage)> LoadPerformanceData()
        {
            var result = new Dictionary<string, (double, double)>();  // Dictionary to hold models and their performance metrics

            // Open the Excel file to read performance metrics
            using (var fileStream = new FileStream(_performanceFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);  // Open Excel workbook
                ISheet sheet = workbook.GetSheetAt(0);  // Get the first sheet

                for (int row = 1; row <= sheet.LastRowNum; row++) // Skip the header row
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    string model = currentRow?.GetCell(0)?.ToString()?.Trim() ?? "";  // Get model name from the first cell

                    // Parse time and memory usage values from the second and third cells
                    double time = 0;
                    var timeCell = currentRow?.GetCell(1);
                    if (timeCell != null && double.TryParse(timeCell.ToString(), out var t))
                    {
                        time = t;
                    }

                    double mem = 0;
                    var memCell = currentRow?.GetCell(2);
                    if (memCell != null && double.TryParse(memCell.ToString(), out var m))
                    {
                        mem = m;
                    }

                    // Add to the result dictionary if model is not empty
                    if (!string.IsNullOrEmpty(model))
                    {
                        result[model] = (time, mem);  // Store model and its performance metrics
                    }
                }
            }

            return result;  // Return the loaded performance data
        }

        /// <summary>
        /// Saves the ranked model evaluation report to an Excel file.
        /// </summary>
        private void SaveReportToExcel(List<(string Model, double FinalScore)> report)
        {
            // Determine the output folder and file name for the report
            string outputFolder = Path.GetDirectoryName(_similarityFilePath) ?? Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(outputFolder, "BestModelRanking.xlsx");

            // Create a new Excel workbook and sheet for the report
            using (var workbook = new XSSFWorkbook())
            {
                ISheet sheet = workbook.CreateSheet("Model Ranking");

                // Create header row
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Rank");
                headerRow.CreateCell(1).SetCellValue("Model");
                headerRow.CreateCell(2).SetCellValue("Final Score");

                // Fill data for each ranked model
                for (int i = 0; i < report.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);  // Start from row 1 (after the header)
                    row.CreateCell(0).SetCellValue(i + 1);  // Rank
                    row.CreateCell(1).SetCellValue(report[i].Model);  // Model name
                    row.CreateCell(2).SetCellValue(report[i].FinalScore);  // Final score
                }

                // Save the workbook to the output file path
                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                Console.WriteLine($"Report saved successfully to: {outputPath}");
            }
        }
    }
}
