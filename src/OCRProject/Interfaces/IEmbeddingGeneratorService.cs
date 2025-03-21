using System;
using System.Collections.Generic;

namespace OCRProject.Interfaces
{
    public interface IEmbeddingGeneratorService
    {
        /// <summary>
        /// Generates numerical embeddings for given text inputs.
        /// </summary>
        /// <param name="extractedTexts">Dictionary with model names as keys and extracted text as values.</param>
        /// <returns>Dictionary with model names as keys and embeddings as float arrays.</returns>
        Dictionary<string, float[]> GenerateEmbeddingsForModels(Dictionary<string, string> extractedTexts);
    }
}