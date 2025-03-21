using System.Collections.Generic;

namespace OCRProject.Interfaces
{
    public interface ICosineSimilarityCalculator
    {
        /// <summary>
        /// Computes cosine similarity between different model embeddings and generates an Excel report.
        /// </summary>
        /// <param name="embeddings">Dictionary with model names and their embeddings.</param>
        void ComputeAndSaveReport(Dictionary<string, float[]> embeddings);
    }
}