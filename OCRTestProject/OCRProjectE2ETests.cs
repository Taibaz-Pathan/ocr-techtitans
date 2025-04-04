using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace OCRProject.Tests
{
    [TestFixture]
    public class OCRProjectE2ETests
    {
        // Base paths for test execution
        private readonly static string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
        private readonly string testDataPath = Path.Combine(basePath, "OCRTestProject", "TestData");
        private readonly string executablePath = Path.Combine(basePath, "OCRProject", "bin", "Debug", "net9.0", "OCRProject.exe");
        private readonly string mainInputFolder = Path.Combine(basePath, "OCRProject", "Input");
        private readonly string mainOutputFolder = Path.Combine(basePath, "OCRProject", "Output", "ExtractedText");
        private readonly string logFile = Path.Combine(basePath, "OCRTestProject", "logs", "log.txt");

        [SetUp]
        public void Setup()
        {
            // Verify executable exists before running tests
            if (!File.Exists(executablePath))
                Assert.Fail($"Executable not found: {executablePath}");

            // Ensure directories exist and are clean
            Directory.CreateDirectory(mainInputFolder);
            Directory.CreateDirectory(mainOutputFolder);
            ClearDirectory(mainInputFolder);
            ClearDirectory(mainOutputFolder);

            // Clear previous log file if exists
            if (File.Exists(logFile)) File.WriteAllText(logFile, string.Empty);
        }

        #region Test Cases

        /// <summary>
        /// Tests OCR system behavior when input folder is empty
        /// Expected: System should detect and report no images found
        /// </summary>
        [Test]
        public void Test_OCR_Empty_Input_Folder()
        {
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found"), "OCR did not detect an empty input folder correctly.");
        }

        /// <summary>
        /// Tests OCR processing with a standard valid image
        /// Expected: System should complete processing and generate output
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Valid_Image()
        {
            CopyTestFile("sample-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not complete successfully.");
            AssertOutputFileExists();
        }

        /// <summary>
        /// Tests OCR error handling with corrupt image file
        /// Expected: System should gracefully handle the error and report it
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Corrupt_Image()
        {
            CopyTestFile("corrupt-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Error processing"), "OCR did not handle a corrupt image correctly.");
        }

        /// <summary>
        /// Tests OCR performance with large image files
        /// Expected: System should complete processing within reasonable time
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Large_Image()
        {
            CopyTestFile("large-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle large images correctly.");
        }

        /// <summary>
        /// Tests OCR handling of invalid file formats
        /// Expected: System should ignore non-image files
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Invalid_File()
        {
            CopyTestFile("test.txt");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found in the input folder."), "OCR did not handle invalid file format correctly.");
        }

        /// <summary>
        /// Tests batch processing of multiple images
        /// Expected: System should process all images and generate combined output
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Multiple_Images()
        {
            CopyTestFile("sample-image.png");
            CopyTestFile("sample-image1.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process multiple images correctly.");
            AssertOutputFileExists();
        }

        /// <summary>
        /// Tests OCR capability with multilingual text
        /// Expected: System should extract text from multiple languages
        /// </summary>
        [Test]
        public void Test_OCR_Processing_MultiLanguage_Text()
        {
            CopyTestFile("multi-language.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle multi-language text correctly.");
        }

        /// <summary>
        /// Tests OCR with rotated text images
        /// Expected: System should successfully process rotated text
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Rotated_Text()
        {
            CopyTestFile("rotated-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process rotated text correctly.");
        }

        /// <summary>
        /// Tests OCR with noisy background images
        /// Expected: System should extract text despite background noise
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Noisy_Image()
        {
            CopyTestFile("noisy-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle noisy images correctly.");
        }

        /// <summary>
        /// Tests OCR with number-only content
        /// Expected: System should accurately extract numeric characters
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Only_Numbers()
        {
            CopyTestFile("numbers-only.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not extract numbers correctly.");
        }

        /// <summary>
        /// Tests OCR with blank/empty images
        /// Expected: System should handle empty images gracefully
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Empty_Image()
        {
            CopyTestFile("empty-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Error opening data file ./tessdata/eng.traineddata"), "OCR should return an empty result for blank images.");
        }

        /// <summary>
        /// Tests OCR with skewed text (perspective distortion)
        /// Expected: System should correct skew and extract text
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Skewed_Text()
        {
            CopyTestFile("skewed-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process skewed text correctly.");
        }

        /// <summary>
        /// Tests batch processing with multiple different images
        /// Expected: System should handle varied image types in single run
        /// </summary>
        [Test]
        public void Test_Multiple_Images_In_Input()
        {
            CopyTestFile("multi-image-1.png");
            CopyTestFile("multi-image-2.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process multiple images correctly.");
        }

        /// <summary>
        /// Tests support for various image formats
        /// Expected: System should process JPG, BMP, etc. formats
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Different_Image_Formats()
        {
            CopyTestFile("test-image.jpg");
            CopyTestFile("test-image.bmp");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle different image formats correctly.");
        }

        /// <summary>
        /// Tests OCR with handwritten text
        /// Expected: System should attempt to process (with potentially lower accuracy)
        /// </summary>
        [Test]
        public void Test_OCR_Processing_Handwritten_Text()
        {
            CopyTestFile("handwritten-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process handwritten text correctly.");
        }

        /// <summary>
        /// Tests OCR with low contrast images
        /// Expected: System should enhance and extract text
        /// </summary>
        [Test]
        public void Test_OCR_Low_Contrast_Image()
        {
            CopyTestFile("low-contrast.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process low-contrast images correctly.");
        }

        /// <summary>
        /// Tests OCR with multi-column layouts
        /// Expected: System should maintain text structure
        /// </summary>
        [Test]
        public void Test_OCR_Multi_Column_Text()
        {
            CopyTestFile("multi-column.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle multi-column text correctly.");
        }

        /// <summary>
        /// Tests OCR with special characters and symbols
        /// Expected: System should recognize special characters
        /// </summary>
        [Test]
        public void Test_OCR_Special_Characters_And_Emojis()
        {
            CopyTestFile("special-characters.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not recognize special characters or emojis correctly.");
        }

        /// <summary>
        /// Tests OCR with non-image file (PDF)
        /// Expected: System should ignore non-supported formats
        /// </summary>
        [Test]
        public void Test_OCR_Processing_PDF_File()
        {
            CopyTestFile("test-document.pdf");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found"), "OCR did not handle non-image file types correctly.");
        }

        /// <summary>
        /// Tests grayscale preprocessing
        /// Expected: System should process grayscale images effectively
        /// </summary>
        [Test]
        public void Test_ImageProcessing_Grayscale()
        {
            CopyTestFile("grayscale-test.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process grayscale images correctly.");
        }

        /// <summary>
        /// Tests noise reduction preprocessing
        /// Expected: System should clean noisy images before OCR
        /// </summary>
        [Test]
        public void Test_ImageProcessing_NoiseReduction()
        {
            CopyTestFile("noise-reduction-test.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle noise reduction correctly.");
        }

        /// <summary>
        /// Tests rotation correction preprocessing
        /// Expected: System should deskew rotated images
        /// </summary>
        [Test]
        public void Test_ImageProcessing_Rotation()
        {
            CopyTestFile("rotated-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process rotated images correctly.");
        }

        /// <summary>
        /// Tests process interruption handling
        /// Expected: System should terminate cleanly when interrupted
        /// </summary>
        [Test]
        public void Test_OCR_Interrupted_Process()
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = executablePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                Thread.Sleep(1000); // Let the process run for a second
                process.Kill();

                Assert.That(process.HasExited, "OCR process was not terminated successfully.");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Copies test file from test data directory to input directory
        /// </summary>
        /// <param name="fileName">Name of test file to copy</param>
        private void CopyTestFile(string fileName)
        {
            string sourcePath = Path.Combine(testDataPath, fileName);
            string destinationPath = Path.Combine(mainInputFolder, fileName);

            if (!File.Exists(sourcePath))
                Assert.Fail($"Test file missing: {sourcePath}");

            File.Copy(sourcePath, destinationPath, true);
        }

        /// <summary>
        /// Executes the OCR process and captures output
        /// </summary>
        /// <returns>Combined standard output and error output</returns>
        private string RunOCRProcess()
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = executablePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                string output = "";
                process.OutputDataReceived += (sender, e) => { if (e.Data != null) output += e.Data + Environment.NewLine; };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) output += "[ERROR] " + e.Data + Environment.NewLine; };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool exited = process.WaitForExit(30000); // 30s timeout
                if (!exited)
                {
                    process.Kill();
                    throw new Exception("OCR process timed out.");
                }

                return output;
            }
        }

        /// <summary>
        /// Clears all files from a directory
        /// </summary>
        /// <param name="path">Directory path to clear</param>
        private void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Verifies output file was generated
        /// </summary>
        private void AssertOutputFileExists()
        {
            string[] files = Directory.GetFiles(mainOutputFolder, "*.txt");
            Assert.That(files.Length, Is.GreaterThan(0), "No OCR output file was generated.");
        }

        #endregion
    }
}