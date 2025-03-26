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

        // Constructor to initialize the service, load API key, and set up HttpClient
        public EmbeddingGeneratorService()
        {
            // Load the API key using ConfigLoader to fetch from app.json file
            var configLoader = new ConfigLoader();
            ApiKey = LoadApiKeyFromConfig(configLoader.appkeypath);  // Load the API key from app.json

            // Initialize the HttpClient and set the Authorization header with the API key
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        /// <summary>
        /// Loads the API key from the app.json file located at the specified path.
        /// Throws an exception if the key is missing.
        /// </summary>
        private static string LoadApiKeyFromConfig(string appKeyPath)
        {
            // Use ConfigurationBuilder to load the API key from app.json directly using the provided appKeyPath
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(appKeyPath, optional: false, reloadOnChange: true)  // Load app.json file
                .Build();

            // Retrieve the API key from the loaded configuration under "Mykey:ApiKey"
            var apiKey = configuration["ApiKeys:OpenAI"];
            if (string.IsNullOrEmpty(apiKey))
            {
                // If the API key is missing or empty, throw an exception to notify the error
                throw new Exception("API key is missing from the configuration file.");
            }

            return apiKey;  // Return the API key if found
        }

        /// <summary>
        /// Generates embeddings for given text inputs using OpenAI API.
        /// </summary>
        /// <param name="extractedTexts">Dictionary with model names as keys and extracted text as values.</param>
        /// <returns>Dictionary with model names as keys and embeddings as float arrays.</returns>
        public async Task<Dictionary<string, float[]>> GenerateEmbeddingsForModelsAsync(Dictionary<string, string> extractedTexts)
        {
            var embeddings = new Dictionary<string, float[]>();  // To store the final embeddings for each model

            // Iterate over each model and its corresponding extracted text
            foreach (var model in extractedTexts)
            {
                // Only process non-empty strings for text input
                if (!string.IsNullOrWhiteSpace(model.Value))
                {
                    // Call OpenAI API to generate the embedding for this text
                    var embedding = await GenerateEmbeddingFromOpenAIAsync(model.Value);
                    embeddings[model.Key] = embedding;  // Store the generated embedding in the dictionary
                }
            }

            return embeddings;  // Return the dictionary of embeddings
        }

        /// <summary>
        /// Calls OpenAI API to generate an embedding for the provided text.
        /// </summary>
        private async Task<float[]> GenerateEmbeddingFromOpenAIAsync(string text)
        {
            // Create request data object for the OpenAI API
            var requestData = new
            {
                model = "text-embedding-ada-002", // Use the appropriate model for embedding
                input = text  // The text input to be embedded
            };

            // Serialize the request data to JSON format
            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Send a POST request to the OpenAI API with the generated JSON content
            var response = await _httpClient.PostAsync(ApiEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();  // Read the API response as a string

            // If the response status is not successful, throw an exception with the error message
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {responseContent}");
            }

            // Deserialize the response content into the OpenAIEmbeddingResponse object
            var responseObject = JsonConvert.DeserializeObject<OpenAIEmbeddingResponse>(responseContent);

            // Check if the embedding data exists in the response and handle any missing values
            if (responseObject?.Data == null || responseObject.Data.Length == 0 || responseObject.Data[0]?.Embedding == null)
            {
                // Return an empty array if no valid embedding data is present to avoid null reference errors
                return Array.Empty<float>();  // Return an empty array instead of null
            }

            // Return the first embedding in the response, which is guaranteed to be non-null
            return responseObject.Data[0].Embedding ?? Array.Empty<float>();  // Use Array.Empty if Embedding is null
        }
    }


    // Response class to represent the OpenAI API response structure
    public class OpenAIEmbeddingResponse
    {
        // Nullable Data array to handle cases where the response may not contain any embedding data
        public OpenAIEmbeddingData[]? Data { get; set; }
    }

    // Data class to represent each embedding returned from OpenAI API
    public class OpenAIEmbeddingData
    {
        // Nullable Embedding array to handle missing or empty embeddings
        public float[]? Embedding { get; set; }
    }
}
