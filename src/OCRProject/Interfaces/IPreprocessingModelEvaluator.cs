namespace OCRProject.Interfaces;

/// <summary>
/// Interface for evaluating different preprocessing models and reporting the best-performing ones.
/// </summary>
public interface IPreprocessingModelEvaluator
{
    /// <summary>
    /// Evaluates multiple models and generates a report on the best-performing ones.
    /// </summary>
    void EvaluateAndReportBestModels();
}

/// <summary>
/// Interface for loading data related to model evaluation, including similarity scores and performance metrics.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Loads cosine similarity scores from a specified file.
    /// </summary>
    /// <param name="filePath">The path to the file containing cosine similarity data.</param>
    /// <returns>A dictionary where the key is the model name, and the value is the cosine similarity score.</returns>
    Dictionary<string, double> LoadCosineSimilarity(string filePath);

    /// <summary>
    /// Loads performance data such as time taken and memory usage from a specified file.
    /// </summary>
    /// <param name="filePath">The path to the file containing performance data.</param>
    /// <returns>A dictionary where the key is the model name, and the value is a tuple containing time taken and memory usage.</returns>
    Dictionary<string, (double TimeTaken, double MemoryUsage)> LoadPerformanceData(string filePath);
}

/// <summary>
/// Interface for generating reports and saving them to an Excel file.
/// </summary>
public interface IReportGenerator
{
    /// <summary>
    /// Saves a report containing model evaluation results to an Excel file.
    /// </summary>
    /// <param name="report">A list of tuples where each tuple contains a model name and its final evaluation score.</param>
    /// <param name="outputPath">The file path where the Excel report will be saved.</param>
    void SaveReportToExcel(List<(string Model, double FinalScore)> report, string outputPath);
}