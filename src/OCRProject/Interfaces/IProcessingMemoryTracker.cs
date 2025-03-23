namespace OCRProject.ModelComparision;

/// <summary>
/// Evaluates preprocessing models and reports the best ones.
/// </summary>
public interface IPreprocessingModelEvaluator
{
    void EvaluateAndReportBestModels();
}

/// <summary>
/// Loads similarity scores and performance data from files.
/// </summary>
public interface IDataLoader
{
    Dictionary<string, double> LoadCosineSimilarity(string filePath);
    Dictionary<string, (double TimeTaken, double MemoryUsage)> LoadPerformanceData(string filePath);
}

/// <summary>
/// Generates reports and saves them to an Excel file.
/// </summary>
public interface IReportGenerator
{
    void SaveReportToExcel(List<(string Model, double FinalScore)> report, string outputPath);
}

/// <summary>
/// Tracks and logs memory usage during image processing.
/// </summary>
public interface IProcessingMemoryTracker
{
    void MeasureMemoryUsage(string imageName, string processingStep, Action action);
    void AppendMemoryUsageToExcel();
}