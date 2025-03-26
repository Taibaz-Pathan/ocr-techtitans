using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparision
{
    // Class to generate embeddings using Microsoft ML.NET
    public class MLEmbeddingGeneratorService : IMLEmbeddingGeneratorService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _trainedModel;
        private readonly DataViewSchema _schema;

        // Constructor initializes MLContext and sets up the model pipeline for text featurization
        public MLEmbeddingGeneratorService()
        {
            _mlContext = new MLContext();  // Initializes ML.NET context for all operations

            // Define a pipeline for text normalization, tokenization, and word embedding
            var pipeline = _mlContext.Transforms.Text.NormalizeText("NormalizedText", "Text")  // Normalize the input text
                .Append(_mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "NormalizedText"))  // Tokenize text into words
                .Append(_mlContext.Transforms.Text.ApplyWordEmbedding("Features", "Tokens",  // Apply pretrained word embeddings
                        Microsoft.ML.Transforms.Text.WordEmbeddingEstimator.PretrainedModelKind.SentimentSpecificWordEmbedding));  // Use a sentiment-specific pretrained embedding model

            // Create an empty data set to fit the pipeline (for model initialization)
            var emptyData = new List<TextData>();
            var dataView = _mlContext.Data.LoadFromEnumerable(emptyData);  // Load the empty data into the pipeline
            _trainedModel = pipeline.Fit(dataView);  // Fit the pipeline to the empty data (initialize model)
            _schema = _trainedModel.GetOutputSchema(dataView.Schema);  // Get the schema of the output data
        }

        /// <summary>
        /// Asynchronously generates embeddings for multiple text inputs.
        /// </summary>
        /// <param name="extractedTexts">A dictionary of model names as keys and extracted text as values.</param>
        /// <returns>A dictionary with model names as keys and their corresponding embeddings as float arrays.</returns>
        public async Task<Dictionary<string, float[]>> GenerateEmbeddingsForModelsAsync(Dictionary<string, string> extractedTexts)
        {
            var embeddings = new Dictionary<string, float[]>();  // Dictionary to store model embeddings

            // Iterate through each model and its corresponding extracted text
            foreach (var model in extractedTexts)
            {
                if (!string.IsNullOrWhiteSpace(model.Value))  // Skip empty or null texts
                {
                    // Generate the embedding for the current text
                    var embedding = GenerateEmbedding(model.Value);
                    embeddings[model.Key] = embedding;  // Store the generated embedding in the dictionary
                }
            }

            return await Task.FromResult(embeddings);  // Return the embeddings asynchronously
        }

        /// <summary>
        /// Generates an embedding for a given text using the trained ML model.
        /// </summary>
        private float[] GenerateEmbedding(string text)
        {
            // Load the input text into a DataView (the standard data format in ML.NET)
            var dataView = _mlContext.Data.LoadFromEnumerable(new List<TextData> { new() { Text = text } });
            // Transform the input text into feature vectors using the trained model
            var transformedData = _trainedModel.Transform(dataView);
            // Extract the features (embeddings) from the transformed data
            var predictions = _mlContext.Data.CreateEnumerable<TextEmbedding>(transformedData, reuseRowObject: false).FirstOrDefault();

            // Return the extracted features or an empty array if no features are found
            return predictions?.Features ?? Array.Empty<float>();
        }
    }

    // Class to represent the input data (text) for the ML pipeline
    public class TextData
    {
        public string Text { get; set; } = string.Empty;  // The input text to be processed
    }

    // Class to represent the output of the ML model, containing the generated features (embeddings)
    public class TextEmbedding
    {
        [VectorType(600)]  // Specifies that the "Features" property is a vector with a fixed size of 600 elements
        public float[] Features { get; set; } = Array.Empty<float>();  // The generated embedding (feature vector)
    }
}
