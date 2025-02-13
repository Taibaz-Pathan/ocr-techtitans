using System;
using OpenAI;

namespace OCRProject.TesseractProcessor
{
    public class GPTTextExtractor
    {
        public string ExtractText(string imagePath)
        {
            var MyApiKey = "";
            var api = new OpenAI_API.APIAuthentication(MyApiKey);
            var openAI = new OpenAI_API.OpenAIAPI(api);

            var result = openAI.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest(
                prompt: $"Extract text from image at path: {imagePath}",
                model: "gpt-3.5-turbo-instruct", // Updated model as text-davinci-003 is deprecated
                max_tokens: 500
            )).Result;

            string extractedText = result.Completions[0].Text;
            Console.WriteLine("Extracted Text from GPT: " + extractedText);
            return extractedText;
        }
    }
}