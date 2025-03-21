namespace OCRProject.ModelComparision;

public interface IPreprocessingModelEvaluator
{
    void EvaluateAndReportBestModels();
}
    
public interface IDataLoader
{
    Dictionary<string, double> LoadCosineSimilarity(string filePath);
    Dictionary<string, (double TimeTaken, double MemoryUsage)> LoadPerformanceData(string filePath);
}
    
public interface IReportGenerator
{
    void SaveReportToExcel(List<(string Model, double FinalScore)> report, string outputPath);
}
    
public interface IProcessingMemoryTracker
{
    void MeasureMemoryUsage(string imageName, string processingStep, Action action);
    void AppendMemoryUsageToExcel();
}