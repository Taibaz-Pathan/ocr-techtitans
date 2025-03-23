using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OCRProject.Utils;  // Reference to ConfigLoader
using Newtonsoft.Json;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparision
{
    public class EmbeddingGeneratorService : IEmbeddingGeneratorService 
    {
        private readonly string ApiKey;
        private const string ApiEndpoint = "https://api.openai.com/v1/embeddings";
        private readonly HttpClient _httpClient;

        public EmbeddingGeneratorService()
        {
            // Load the API key using ConfigLoader
            var configLoader = new ConfigLoader();
            ApiKey = LoadApiKeyFromConfig(configLoader.appkeypath);  // Load the API key from app.json

            // Initialize HttpClient
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        /// <summary>
        /// Loads the API key from the app.json file located at the specified path.
        /// Throws an exception if the key is missing.
        /// </summary>
        private static string LoadApiKeyFromConfig(string appKeyPath)
        {
            // Use the ConfigurationBuilder to load the API key from the app.json file directly using appkeypath
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(appKeyPath, optional: false, reloadOnChange: true)  // Directly use appkeypath
                .Build();

            // Retrieve the API key from the loaded configuration
            var apiKey = configuration["Mykey:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                // Throw an exception if the API key is missing or empty
                throw new Exception("API key is missing from the configuration file.");
            }

            return apiKey;  // Return the API key if it's found
        }

        /// <summary>
        /// Generates embeddings for given text inputs using OpenAI API.
        /// </summary>
        /// <param name="extractedTexts">Dictionary with model names as keys and extracted text as values.</param>
        /// <returns>Dictionary with model names as keys and embeddings as float arrays.</returns>
        public async Task<Dictionary<string, float[]>> GenerateEmbeddingsForModelsAsync(Dictionary<string, string> extractedTexts)
        {
            var embeddings = new Dictionary<string, float[]>();

            foreach (var model in extractedTexts)
            {
                if (!string.IsNullOrWhiteSpace(model.Value))
                {
                    var embedding = await GenerateEmbeddingFromOpenAIAsync(model.Value);
                    embeddings[model.Key] = embedding;
                }
            }

            return embeddings;
        }

        /// <summary>
        /// Calls OpenAI API to generate an embedding for the provided text.
        /// </summary>
        private async Task<float[]> GenerateEmbeddingFromOpenAIAsync(string text)
        {
            var requestData = new
            {
                model = "text-embedding-ada-002", // Use the appropriate model for embedding
                input = text
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {responseContent}");
            }

            // Deserialize response into an object
            var responseObject = JsonConvert.DeserializeObject<OpenAIEmbeddingResponse>(responseContent);

            // Null checks to ensure Data and Embedding are not null
            if (responseObject?.Data == null || responseObject.Data.Length == 0 || responseObject.Data[0]?.Embedding == null)
            {
                // Return an empty array if embedding is missing to prevent null reference.
                return Array.Empty<float>();  // Return an empty array instead of null
            }

            // Return the first embedding (now guaranteed to be non-null)
            return responseObject.Data[0].Embedding ?? Array.Empty<float>(); // Use Array.Empty if Embedding is null
        }
    }

    // These classes should be outside EmbeddingGeneratorService and within the same namespace.
    public class OpenAIEmbeddingResponse
    {
        // Nullable Data because the response could be empty or missing
        public OpenAIEmbeddingData[]? Data { get; set; }
    }

    public class OpenAIEmbeddingData
    {
        // Nullable Embedding because the response could be missing or empty
        public float[]? Embedding { get; set; }
    }
}
