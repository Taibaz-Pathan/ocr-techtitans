using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCRProject.Interfaces
{
    /// <summary>
    /// Defines the contract for generating embeddings for multiple text inputs.
    /// </summary>
    public interface IMLEmbeddingGeneratorService
    {
        /// <summary>
        /// Asynchronously generates embeddings for multiple text inputs.
        /// </summary>
        /// <param name="extractedTexts">A dictionary of model names as keys and extracted text as values.</param>
        /// <returns>A dictionary with model names as keys and their corresponding embeddings as float arrays.</returns>
        Task<Dictionary<string, float[]>> GenerateEmbeddingsForModelsAsync(Dictionary<string, string> extractedTexts);
    }
}
