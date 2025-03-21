namespace OCRProject.Interfaces;

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