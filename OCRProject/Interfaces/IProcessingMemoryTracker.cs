using System;
using System.Collections.Generic;

namespace OCRProject.Interfaces
{
    /// <summary>
    /// Defines the contract for tracking memory usage during processing.
    /// </summary>
    public interface IProcessingMemoryTracker
    {
        /// <summary>
        /// Measures memory usage during a specific action for a given model.
        /// </summary>
        /// <param name="modelName">The name of the model for which memory usage is tracked.</param>
        /// <param name="action">The action to perform while measuring memory usage.</param>
        void MeasureMemoryUsage(string modelName, Action action);

        /// <summary>
        /// Retrieves the last recorded memory usage for a given model.
        /// </summary>
        /// <param name="modelName">The name of the model.</param>
        /// <returns>The last recorded memory usage in MB.</returns>
        double GetLastMemoryUsage(string modelName);

        /// <summary>
        /// Computes and returns the average memory usage for each model.
        /// </summary>
        /// <returns>A dictionary with model names as keys and their average memory usage in MB as values.</returns>
        Dictionary<string, double> GetAverageMemoryResults();
    }
}
