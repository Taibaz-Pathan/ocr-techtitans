using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ModelComparison
{
    public class CosineSimilarityCalculator
    {
        private readonly string _outputFolder;

        public CosineSimilarityCalculator(string outputFolder)
        {
            _outputFolder = outputFolder;
        }

        /// <summary>
        /// Computes cosine similarity between different model embeddings and generates an Excel report.
        /// </summary>
        /// <param name="embeddings">Dictionary with model names and their embeddings.</param>
        public void ComputeAndSaveReport(Dictionary<string, float[]> embeddings)
        {
            if (embeddings.Count < 2)
            {
                Console.WriteLine("Not enough models to compare.");
                return;
            }

            // Normalize embeddings and remove negatives (if needed)
            var normalizedEmbeddings = embeddings.ToDictionary(
                kvp => kvp.Key,
                kvp => NormalizeVector(kvp.Value)
            );

            string outputFile = Path.Combine(_outputFolder, "CosineSimilarity.xlsx");

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Similarity Report");

            // Create header row
            int rowIdx = 0;
            IRow headerRow = sheet.CreateRow(rowIdx++);
            headerRow.CreateCell(0).SetCellValue("Model");

            int colIdx = 1;
            foreach (var model in normalizedEmbeddings.Keys)
            {
                headerRow.CreateCell(colIdx++).SetCellValue(model);
            }

            // Compute similarity for each model pair
            foreach (var modelA in normalizedEmbeddings)
            {
                IRow row = sheet.CreateRow(rowIdx++);
                row.CreateCell(0).SetCellValue(modelA.Key);
                colIdx = 1;

                foreach (var modelB in normalizedEmbeddings)
                {
                    float similarity = ComputeCosineSimilarity(modelA.Value, modelB.Value);
                    row.CreateCell(colIdx++).SetCellValue(similarity);
                }
            }

            // Save report to file
            using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }

            Console.WriteLine($"Report saved to {outputFile}");
        }

        /// <summary>
        /// Computes the cosine similarity between two normalized vectors.
        /// </summary>
        /// <param name="vectorA">First vector.</param>
        /// <param name="vectorB">Second vector.</param>
        /// <returns>Cosine similarity value between 0 and 1, rounded to 6 decimal places.</returns>
        private float ComputeCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            float dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();

            // Ensure similarity is within 0 and 1
            return (float)Math.Round(Math.Max(dotProduct, 0), 6);
        }

        /// <summary>
        /// Normalizes a vector and ensures non-negative values.
        /// </summary>
        /// <param name="vector">The input vector.</param>
        /// <returns>Normalized and non-negative vector.</returns>
        private float[] NormalizeVector(float[] vector)
        {
            // Take absolute values to avoid negative similarities
            vector = vector.Select(x => Math.Abs(x)).ToArray();

            float magnitude = (float)Math.Sqrt(vector.Sum(x => x * x));
            if (magnitude == 0) return new float[vector.Length];  // Avoid division by zero

            return vector.Select(x => x / magnitude).ToArray();
        }
    }
}
