using System;
using System.Collections.Generic;
using System.Diagnostics;
using OCRProject.Interfaces;

namespace OCRProject.ModelComparision
{
    // This class is responsible for tracking memory usage during processing
    public class ProcessingMemoryTracker : IProcessingMemoryTracker
    {
        // Dictionary to store memory usage data for each model (modelName -> list of memory usage values)
        private readonly Dictionary<string, List<double>> _memoryResults = new();

        /// <summary>
        /// Measures memory usage of a given transformation process.
        /// This method will track how much memory is used during a specific action.
        /// </summary>
        public void MeasureMemoryUsage(string modelName, Action action)
        {
            // Get memory usage before the action starts
            long before = GC.GetTotalMemory(true);

            // Perform the action (e.g., image processing)
            action();

            // Get memory usage after the action is completed
            long after = GC.GetTotalMemory(true);

            // Calculate the difference in memory usage and convert it to MB
            double memoryUsedMB = (after - before) / (1024.0 * 1024.0);

            // If the model is not already in the dictionary, add it with a new list for memory usage
            if (!_memoryResults.ContainsKey(modelName))
                _memoryResults[modelName] = new List<double>();

            // Add the measured memory usage for this model
            _memoryResults[modelName].Add(memoryUsedMB);
        }

        /// <summary>
        /// Retrieves the last recorded memory usage for a given model.
        /// This will return the most recent memory usage recorded for the specified model.
        /// </summary>
        public double GetLastMemoryUsage(string modelName)
        {
            // Check if the model exists in the dictionary and has recorded memory usage
            return _memoryResults.ContainsKey(modelName) && _memoryResults[modelName].Count > 0
                // If so, return the last recorded memory value
                ? _memoryResults[modelName].Last()
                // If not, return 0 (indicating no memory usage recorded yet)
                : 0;
        }

        /// <summary>
        /// Computes and returns average memory usage for each model.
        /// This method will calculate the average of all recorded memory usage values for each model.
        /// </summary>
        public Dictionary<string, double> GetAverageMemoryResults()
        {
            // Create a new dictionary to store the average memory usage for each model
            var averageResults = new Dictionary<string, double>();

            // Loop through each model and its list of memory usage values
            foreach (var (model, memoryUsage) in _memoryResults)
            {
                // For each model, calculate the average of its memory usage values
                // If no memory values are recorded, set the average to 0
                averageResults[model] = memoryUsage.Count > 0 ? memoryUsage.Average() : 0;
            }

            // Return the dictionary containing the average memory usage for each model
            return averageResults;
        }
    }
}
