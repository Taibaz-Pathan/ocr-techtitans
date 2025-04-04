using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCRProject.Interfaces
{
    public interface IEmbeddingGeneratorService
    {
        /// <summary>
        /// Generates embeddings for given text inputs using OpenAI API.
        /// </summary>
        /// <param name="extractedTexts">Dictionary with model names as keys and extracted text as values.</param>
        /// <returns>Dictionary with model names as keys and embeddings as float arrays.</returns>
        Task<Dictionary<string, float[]>> GenerateEmbeddingsForModelsAsync(Dictionary<string, string> extractedTexts);
    }
}
