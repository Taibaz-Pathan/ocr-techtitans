using System;
using System.Collections.Generic;
using System.IO;
using OCRProject.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace ModelComparison
{
    public class CosineSimilarityCalculator : ICosineSimilarityCalculator
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

            string outputFile = Path.Combine(_outputFolder, "CosineSimilarity.xlsx");

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Similarity Report");

            int rowIdx = 0;
            IRow headerRow = sheet.CreateRow(rowIdx++);
            headerRow.CreateCell(0).SetCellValue("Model");

            int colIdx = 1;
            foreach (var model in embeddings.Keys)
            {
                headerRow.CreateCell(colIdx++).SetCellValue(model);
            }

            foreach (var modelA in embeddings)
            {
                IRow row = sheet.CreateRow(rowIdx++);
                row.CreateCell(0).SetCellValue(modelA.Key);
                colIdx = 1;
                foreach (var modelB in embeddings)
                {
                    row.CreateCell(colIdx++).SetCellValue(ComputeCosineSimilarity(modelA.Value, modelB.Value));
                }
            }

            using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }

            Console.WriteLine($"Report saved to {outputFile}");
        }

        private float ComputeCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            float dotProduct = 0f;
            float magnitudeA = 0f;
            float magnitudeB = 0f;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            magnitudeA = (float)Math.Sqrt(magnitudeA);
            magnitudeB = (float)Math.Sqrt(magnitudeB);

            return (magnitudeA == 0 || magnitudeB == 0) ? 0 : dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
