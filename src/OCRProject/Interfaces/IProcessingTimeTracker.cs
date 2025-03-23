using System;
using System.Collections.Generic;

namespace OCRProject.Interfaces;

/// <summary>
/// Tracks and records processing time for different image processing steps.
/// </summary>
public interface IProcessingTimeTracker
{
    /// <summary>
    /// Starts the timer for measuring processing time.
    /// </summary>
    void StartTimer();

    /// <summary>
    /// Stops the timer and records the time taken for a specific processing step.
    /// </summary>
    /// <param name="imageName">The name of the image being processed.</param>
    /// <param name="processingStep">The processing step being measured.</param>
    void StopAndRecord(string imageName, string processingStep);

    /// <summary>
    /// Generates an Excel report with recorded processing times.
    /// </summary>
    void GenerateExcelReport();
}