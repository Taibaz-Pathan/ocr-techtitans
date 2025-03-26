using OCRProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OCRProject.ModelComparision
{
    /// <summary>
    /// Tracks the processing time for different models by recording elapsed times.
    /// </summary>
    public class ProcessingTimeTracker : IProcessingTimeTracker
    {
        private readonly Dictionary<string, List<double>> _timeRecords = new(); // Stores a list of times for each model
        private readonly Dictionary<string, Stopwatch> _timers = new(); // Stores a stopwatch for each model to measure elapsed time

        /// <summary>
        /// Starts a timer for a specific model.
        /// </summary>
        /// <param name="modelName">The name of the model for which the timer is started.</param>
        public void StartTimer(string modelName)
        {
            // If no stopwatch is found for the model, create a new one and start it.
            if (!_timers.ContainsKey(modelName))
                _timers[modelName] = new Stopwatch();

            // Restart the stopwatch to measure from the beginning.
            _timers[modelName].Restart();
        }

        /// <summary>
        /// Stops the timer for a specific model and records the elapsed time in milliseconds.
        /// </summary>
        /// <param name="modelName">The name of the model for which the timer is stopped.</param>
        /// <returns>The elapsed time in milliseconds.</returns>
        public double StopAndRecord(string modelName)  // Now returns double
        {
            // If the timer exists for the model, stop it and record the elapsed time.
            if (_timers.ContainsKey(modelName))
            {
                _timers[modelName].Stop(); // Stop the stopwatch
                double elapsedMs = _timers[modelName].Elapsed.TotalMilliseconds; // Get the elapsed time in milliseconds

                // If no time records exist for this model, create a new list to hold the times.
                if (!_timeRecords.ContainsKey(modelName))
                    _timeRecords[modelName] = new List<double>();

                // Add the recorded time to the list of times for the model.
                _timeRecords[modelName].Add(elapsedMs);

                return elapsedMs;  // Return the elapsed time in milliseconds
            }

            // If the model doesn't have a timer, return 0 as default.
            return 0;
        }

        /// <summary>
        /// Gets the average processing time for each model.
        /// </summary>
        /// <returns>A dictionary with model names as keys and their average processing times in milliseconds as values.</returns>
        public Dictionary<string, double> GetAverageTimes()
        {
            // For each model, calculate the average time from the recorded times and return the result.
            return _timeRecords.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average());
        }
    }
}
