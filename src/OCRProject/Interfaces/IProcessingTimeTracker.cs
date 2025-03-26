namespace OCRProject.Interfaces
{
    /// <summary>
    /// Defines the contract for tracking processing times of different models.
    /// </summary>
    public interface IProcessingTimeTracker
    {
        /// <summary>
        /// Starts a timer for a specific model.
        /// </summary>
        /// <param name="modelName">The name of the model for which the timer is started.</param>
        void StartTimer(string modelName);

        /// <summary>
        /// Stops the timer for a specific model and records the elapsed time.
        /// </summary>
        /// <param name="modelName">The name of the model for which the timer is stopped.</param>
        /// <returns>The elapsed time in milliseconds.</returns>
        double StopAndRecord(string modelName);

        /// <summary>
        /// Gets the average processing time for each model.
        /// </summary>
        /// <returns>A dictionary with model names as keys and their average processing times in milliseconds as values.</returns>
        Dictionary<string, double> GetAverageTimes();
    }
}
