using System;
using System.Collections.Generic;
using OCRProject.Interfaces;

namespace ModelComparison
{
    public class EmbeddingGeneratorService : IEmbeddingGeneratorService
    {
        private readonly Random _random;
        private const int EmbeddingSize = 1536; // Simulating OpenAI embedding size

        public EmbeddingGeneratorService()
        {
            _random = new Random();
        }

        /// <summary>
        /// Generates random numerical embeddings for given text inputs.
        /// </summary>
        /// <param name="extractedTexts">Dictionary with model names as keys and extracted text as values.</param>
        /// <returns>Dictionary with model names as keys and embeddings as float arrays.</returns>
        public Dictionary<string, float[]> GenerateEmbeddingsForModels(Dictionary<string, string> extractedTexts)
        {
            var embeddings = new Dictionary<string, float[]>();

            foreach (var model in extractedTexts)
            {
                if (!string.IsNullOrWhiteSpace(model.Value))
                {
                    embeddings[model.Key] = GenerateRandomEmbedding();
                }
            }

            return embeddings;
        }

        
        /// Generates a random numerical embedding vector.
        /// <returns>Array of floating-point numbers representing the embedding.</returns>
        private float[] GenerateRandomEmbedding()
        {
            float[] embedding = new float[EmbeddingSize];

            for (int i = 0; i < EmbeddingSize; i++)
            {
                embedding[i] = (float)(_random.NextDouble() * 2 - 1); // Values between -1 and 1
            }

            return embedding;
        }
    }
}
