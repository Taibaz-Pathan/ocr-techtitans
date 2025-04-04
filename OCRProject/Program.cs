using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using OCRProject.ImageProcessing;
using OCRProject.Utils;
using OCRProject.TesseractProcessor;
using OCRProject.ModelComparision;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.Formats;

class Program
{
    // Allowed image file extensions for processing
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" };

    // Main entry point for the application
    static async Task Main()
    {
        // Initialize various services and trackers
        var logger = new Logger();  // For logging messages
        var embeddingGenerator = new EmbeddingGeneratorService();  // Service to generate model embeddings
        var config = new ConfigLoader();  // Load configuration settings

        // Initialize time and memory trackers
        var timeTracker = new ProcessingTimeTracker();
        var memoryTracker = new ProcessingMemoryTracker();

        // Path to store the final processing results in an Excel file
        string resultsFilePath = Path.Combine(config.ComparisionFolder, "ProcessingResults.xlsx");
        string comparisonResults = config.ComparisionFolder;

        // Cosine Similarity Calculator for model comparison
        var similarityCalculator = new CosineSimilarityCalculator(comparisonResults);

        // Log and notify user that image processing is starting
        Console.WriteLine("Starting Image Processing...");
        logger.LogInfo("Starting Image Processing...");

        // Clean up the directories to prepare for new results
        var directoryCleaner = new DirectoryCleaner(logger);
        directoryCleaner.CleanDirectory(config.OutputImageFolder);
        directoryCleaner.CleanDirectory(config.ExtractedTextFolder);
        directoryCleaner.CleanDirectory(config.ComparisionFolder);

        // Initialize file writer and prepare the text file for storing extracted text
        var fileWriter = new FileWriter();
        string fileNameExtracted = $"ProcessedFile_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string createdFilePath = new CreateFiles().CreateTextFile(config.ExtractedTextFolder, fileNameExtracted);

        // Handle error if the text file creation fails
        if (string.IsNullOrEmpty(createdFilePath))
        {
            logger.LogError("Failed to create text file.");
            Console.WriteLine("Failed to create text file.");
            Environment.Exit(1);  // Exit the program if file creation fails
        }

        logger.LogInfo($"Text file created successfully: {createdFilePath}");

        // Get a list of image files in the input folder, filtering by allowed extensions
        var imageFiles = Directory.GetFiles(config.InputFolder, "*.*")
            .Where(file => AllowedExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToArray();

        // If no images found, log and exit
        if (imageFiles.Length == 0)
        {
            logger.LogWarning("No images found in the input folder.");
            Console.WriteLine("No images found in the input folder.");
            Environment.Exit(0);
        }

        logger.LogInfo($"Processing {imageFiles.Length} images...");

        // Dictionary to store extracted text by model
        var aggregatedTextByModel = new Dictionary<string, string>();

        // Dictionaries to store cumulative processing time and memory usage for each model
        var cumulativeTime = new Dictionary<string, List<double>>();
        var cumulativeMemory = new Dictionary<string, List<double>>();

        // Loop through each image file for processing
        foreach (var inputFilePath in imageFiles)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);

                // Load and resize image
                using var inputImage = Image.Load<Rgba32>(inputFilePath);
                using var resizedImage = ResizeImage(inputImage, 2000, 2000);
                
                // Define the different transformations to apply to the image
                var transformations = new Dictionary<string, Func<Image<Rgba32>, Image<Rgba32>>>()
                {
                   { "Grayscale", img => new ConvertToGrayscale().Apply(img).CloneAs<Rgba32>() },
                   { "GlobalThreshold_80", img => new GlobalThresholding(80).ApplyThreshold(img) },
                   { "GlobalThreshold_100", img => new GlobalThresholding(100).ApplyThreshold(img) },
                   { "GlobalThreshold_110", img => new GlobalThresholding(110).ApplyThreshold(img) },
                   { "GlobalThreshold_120", img => new GlobalThresholding(120).ApplyThreshold(img) },
                   { "GlobalThreshold_140", img => new GlobalThresholding(140).ApplyThreshold(img) },
                   { "GlobalThreshold_150", img => new GlobalThresholding(170).ApplyThreshold(img) },
                   { "SaturationAdjusted_1.2", img => new SaturationAdjustment().Apply(img, 1.2f) },
                   { "SaturationAdjusted_0.6", img => new SaturationAdjustment().Apply(img, 0.6f) },
                   { "SaturationAdjusted_0.9", img => new SaturationAdjustment().Apply(img, 0.9f) },
                   { "SaturationAdjusted_1.6", img => new SaturationAdjustment().Apply(img, 1.6f) },
                   { "SaturationAdjusted_2", img => new SaturationAdjustment().Apply(img, 2f) },
                   { "SaturationAdjusted_2.5", img => new SaturationAdjustment().Apply(img, 2.5f) },
                   { "Deskewed", img => new Deskew().Apply(img) },
                   { "AdaptiveThreshold", img => new AdaptiveThreshold(blockSize: 11, c: 5.0).ApplyThreshold(img) },
                    //{ "Shifted", img => new ShiftImage().Apply(img, -200, -200)    }, // Shift image
                };

                // Loop through each transformation and apply it to the image
                foreach (var (modelName, transform) in transformations)
                {
                    Image<Rgba32>? processedImage = null;

                    // Track time and memory usage for each transformation
                    timeTracker.StartTimer(modelName);
                    memoryTracker.MeasureMemoryUsage(modelName, () =>
                    {
                        processedImage = transform(resizedImage.Clone());  // Apply the transformation
                    });

                    // Get the elapsed time and memory used for the transformation
                    double elapsedTime = timeTracker.StopAndRecord(modelName);
                    var averageMemoryResults = memoryTracker.GetAverageMemoryResults();
                    double memoryUsed = averageMemoryResults.ContainsKey(modelName) ? averageMemoryResults[modelName] : 0;

                    // If the processed image is null, log an error
                    if (processedImage == null)
                        throw new InvalidOperationException("Image transformation returned null.");

                    // Save the processed image to the output folder
                    string outputPath = Path.Combine(config.OutputImageFolder, $"{fileName}_{modelName}.png");
                    processedImage.Save(outputPath);
                    logger.LogInfo($"Saved processed image: {outputPath}");

                    // Extract text from the processed image using OCR (Tesseract)
                    string extractedText = TesseractProcessor.ExtractTextFromImage(processedImage, modelName, createdFilePath, fileWriter);

                    // Store the extracted text for each model
                    if (!aggregatedTextByModel.ContainsKey(modelName))
                    {
                        aggregatedTextByModel[modelName] = extractedText;
                    }
                    else
                    {
                        aggregatedTextByModel[modelName] += "\n" + extractedText;
                    }

                    // Track cumulative time and memory for each transformation
                    if (!cumulativeTime.ContainsKey(modelName))
                    {
                        cumulativeTime[modelName] = new List<double>();
                        cumulativeMemory[modelName] = new List<double>();
                    }
                    cumulativeTime[modelName].Add(elapsedTime);
                    cumulativeMemory[modelName].Add(memoryUsed);
                }
            }
            catch (Exception ex)
            {
                // Log error if any issues occur during processing
                logger.LogError($"Error processing {inputFilePath}: {ex.Message}");
                Console.WriteLine($"Error processing {inputFilePath}: {ex.Message}");
            }
        }

        // Log the extracted text for each model
        logger.LogInfo("Extracted Text for Each Transformation:\n");

        // Generate embeddings from the aggregated text for each model
        var embeddings = await embeddingGenerator.GenerateEmbeddingsForModelsAsync(aggregatedTextByModel);

        // Compute cosine similarity between the models and save the report
        similarityCalculator.ComputeAndSaveReport(embeddings);

        // Compute average processing time and memory usage for each model
        var finalTimeResults = cumulativeTime.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average());
        var finalMemoryResults = cumulativeMemory.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average());

        // Save the results in an Excel file
        SaveResultsToExcel(resultsFilePath, finalTimeResults, finalMemoryResults);

        Console.WriteLine("Waiting for ProcessingResults.xlsx to be completely saved...");

        // Paths to similarity and performance result files
        string similarityFilePath = Path.Combine(comparisonResults, "CosineSimilarity.xlsx");
        string performanceFilePath = Path.Combine(comparisonResults, "ProcessingResults.xlsx");

        Console.WriteLine($"Results saved in {resultsFilePath}");

        // If the necessary files exist, evaluate the best models
        if (File.Exists(similarityFilePath) && File.Exists(performanceFilePath))
        {
            Console.WriteLine("Evaluating best models...");
            var evaluator = new PreprocessingModelEvaluator(similarityFilePath, performanceFilePath);
            evaluator.EvaluateAndReportBestModels();
        }
        else
        {
            Console.WriteLine("Skipping model evaluation: Required files not found.");
        }

        Console.WriteLine($"Processing completed");
    }

    // Helper method to adjust image saturation
    private static Image<Rgba32> ApplySaturationAdjustment(Image<Rgba32> image, float saturation)
    {
        image.Mutate(x => x.Saturate(saturation));  // Apply saturation adjustment
        return image;
    }

    // Helper method to resize image while maintaining aspect ratio
    private static Image<Rgba32> ResizeImage(Image<Rgba32> image, int maxWidth, int maxHeight)
    {
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,  // Maintain aspect ratio
            Size = new Size(maxWidth, maxHeight)
        }));
        return image;
    }

    // Helper method to save the processing results to an Excel file
    private static void SaveResultsToExcel(string filePath, Dictionary<string, double> timeResults, Dictionary<string, double> memoryResults)
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Processing Results");

        // Create header row for the Excel file
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("Model");
        headerRow.CreateCell(1).SetCellValue("Average Time (s)");
        headerRow.CreateCell(2).SetCellValue("Average Memory (MB)");

        // Populate the sheet with results for each model
        int rowIndex = 1;
        foreach (var key in timeResults.Keys)
        {
            IRow row = sheet.CreateRow(rowIndex++);
            row.CreateCell(0).SetCellValue(key);
            row.CreateCell(1).SetCellValue(timeResults[key]);
            row.CreateCell(2).SetCellValue(memoryResults.ContainsKey(key) ? memoryResults[key] : 0);
        }

        // Write the Excel file to disk
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(fileStream);
        }

        workbook.Close();
    }
}
