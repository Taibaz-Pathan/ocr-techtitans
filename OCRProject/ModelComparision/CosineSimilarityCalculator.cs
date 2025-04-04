using System;
using System.Collections.Generic;
using System.IO;
using OCRProject.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace OCRProject.ModelComparision
{
    /// <summary>
    /// Computes cosine similarity between model embeddings and generates an Excel report.
    /// </summary>
    public class CosineSimilarityCalculator : ICosineSimilarityCalculator
    {
        private readonly string _outputFolder;

        // Constructor to initialize the output folder where the report will be saved
        public CosineSimilarityCalculator(string outputFolder)
        {
            _outputFolder = outputFolder;
        }

        /// <summary>
        /// Calculates cosine similarity between models and saves the results in an Excel file.
        /// </summary>
        /// <param name="embeddings">Dictionary containing model names and their embeddings.</param>
        public void ComputeAndSaveReport(Dictionary<string, float[]> embeddings)
        {
            // If there are less than 2 models, we cannot compare them, so exit early
            if (embeddings.Count < 2)
            {
                Console.WriteLine("Not enough models to compare.");
                return;
            }

            // Define the file path to save the Excel report
            string outputFile = Path.Combine(_outputFolder, "CosineSimilarity.xlsx");

            // Create a new workbook and sheet for the report
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Similarity Report");

            // Create the header row in the Excel sheet with model names as column headers
            int rowIdx = 0;
            IRow headerRow = sheet.CreateRow(rowIdx++);
            headerRow.CreateCell(0).SetCellValue("Model");

            int colIdx = 1;
            foreach (var model in embeddings.Keys)
            {
                headerRow.CreateCell(colIdx++).SetCellValue(model);
            }

            // Now compute cosine similarity for each pair of models and populate the sheet
            foreach (var modelA in embeddings)
            {
                IRow row = sheet.CreateRow(rowIdx++);
                row.CreateCell(0).SetCellValue(modelA.Key);  // Set modelA's name in the first column
                colIdx = 1;

                // Compare modelA with every other model (including itself) and fill the similarity values
                foreach (var modelB in embeddings)
                {
                    row.CreateCell(colIdx++).SetCellValue(ComputeCosineSimilarity(modelA.Value, modelB.Value));
                }
            }

            // Save the Excel report to the specified file
            using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }

            // Notify the user that the report has been saved
            Console.WriteLine($"Report saved to {outputFile}");
        }

        /// <summary>
        /// Computes the cosine similarity between two embedding vectors.
        /// </summary>
        private float ComputeCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            float dotProduct = 0f;
            float magnitudeA = 0f;
            float magnitudeB = 0f;

            // Calculate the dot product and magnitudes of both vectors
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            // Take square root of the sum of squares to get the magnitudes
            magnitudeA = (float)Math.Sqrt(magnitudeA);
            magnitudeB = (float)Math.Sqrt(magnitudeB);

            // If either vector has zero magnitude, return 0 (no similarity)
            return (magnitudeA == 0 || magnitudeB == 0) ? 0 : dotProduct / (magnitudeA * magnitudeB);
        }

        /// <summary>
        /// Formats model names for GlobalThresholding and SaturationAdjustment methods.
        /// </summary>
        private string FormatModelName(string modelName)
        {
            // If the model name is for GlobalThresholding, format it accordingly
            if (modelName.StartsWith("GlobalThresholding("))
            {
                return modelName.Replace("GlobalThresholding(", "GlobalThresholding_").TrimEnd(')');
            }

            // If the model name is for SaturationAdjustment, format it similarly
            if (modelName.StartsWith("SaturationAdjustment("))
            {
                return modelName.Replace("SaturationAdjustment(", "SaturationAdjustment_").TrimEnd(')');
            }

            // Return the model name unchanged if it doesn't match any special format
            return modelName;
        }
    }
}
