using System;
using System.Collections.Generic;
using System.Linq;

namespace OCRProject.TesseractProcessor
{
    public class TextToMatrixConverter
    {
        private Dictionary<string, List<List<string>>> textMatrix;

        public TextToMatrixConverter()
        {
            textMatrix = new Dictionary<string, List<List<string>>>();
        }

        // Converts extracted text into a matrix and stores it
        public void StoreTextMatrix(string methodName, string extractedText)
        {
            var matrix = extractedText
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries) // Split into lines
                .Select(line => line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList()) // Split into words
                .ToList();

            if (!textMatrix.ContainsKey(methodName))
            {
                textMatrix[methodName] = new List<List<string>>();
            }

            textMatrix[methodName] = matrix;
        }

        // Retrieve stored matrix for a specific method
        public List<List<string>> GetTextMatrix(string methodName)
        {
            return textMatrix.ContainsKey(methodName) ? textMatrix[methodName] : new List<List<string>>();
        }

        // Save the matrix to a file
        public void SaveMatrixToFile(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                foreach (var method in textMatrix.Keys)
                {
                    writer.WriteLine($"Method: {method}");
                    foreach (var row in textMatrix[method])
                    {
                        writer.WriteLine(string.Join(" | ", row));
                    }
                    writer.WriteLine(); // Separate different methods
                }
            }
        }

        // Print the matrix to the console
        public void PrintMatrixToConsole()
        {
            foreach (var method in textMatrix.Keys)
            {
                Console.WriteLine($"Method: {method}");
                foreach (var row in textMatrix[method])
                {
                    Console.WriteLine(string.Join(" | ", row)); // Print each row with words separated by "|"
                }
                Console.WriteLine(); // Separate different methods
            }
        }
    }
}
