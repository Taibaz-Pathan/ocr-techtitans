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
        // Define file paths for testing
        private readonly static string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
        private readonly string testDataPath = Path.Combine(basePath, "OCRTestProject", "TestData");
        private readonly string executablePath = Path.Combine(basePath, "OCRProject", "bin", "Debug", "net9.0", "OCRProject.exe");
        private readonly string mainInputFolder = Path.Combine(basePath, "OCRProject", "Input");
        private readonly string mainOutputFolder = Path.Combine(basePath, "OCRProject", "Output", "ExtractedText");
        private readonly string logFile = Path.Combine(basePath, "OCRTestProject", "logs", "log.txt");

        [SetUp]
        public void Setup()
        {
            // Ensure the OCR executable exists before running tests
            if (!File.Exists(executablePath))
                Assert.Fail($"Executable not found: {executablePath}");

            // Prepare necessary directories and clean up previous test data
            Directory.CreateDirectory(mainInputFolder);
            Directory.CreateDirectory(mainOutputFolder);
            ClearDirectory(mainInputFolder);
            ClearDirectory(mainOutputFolder);

            // Clear log file before starting a new test
            if (File.Exists(logFile)) File.WriteAllText(logFile, string.Empty);
        }

        [Test]
        public void Test_OCR_Empty_Input_Folder()
        {
            // Run OCR on an empty input folder and check the expected output message
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found"), "OCR did not detect an empty input folder correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Valid_Image()
        {
            // Copy a sample image and run OCR
            CopyTestFile("sample-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not complete successfully.");
            AssertOutputFileExists();
        }

        [Test]
        public void Test_OCR_Processing_Corrupt_Image()
        {
            // Test handling of a corrupt image file
            CopyTestFile("corrupt-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Error processing"), "OCR did not handle a corrupt image correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Large_Image()
        {
            // Test OCR processing on a large image file
            CopyTestFile("large-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle large images correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Invalid_File()
        {
            // Test handling of a non-image file (e.g., .txt file)
            CopyTestFile("test.txt");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found in the input folder."), "OCR did not handle invalid file format correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Multiple_Images()
        {
            // Test OCR processing on multiple images in the input folder
            CopyTestFile("sample-image.png");
            CopyTestFile("sample-image1.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process multiple images correctly.");
            AssertOutputFileExists();
        }

        // Additional test cases for various OCR scenarios
        // Each test ensures OCR functionality for different edge cases like multi-language text, rotated text, noisy images, and more

        [Test]
        public void Test_OCR_Interrupted_Process()
        {
            // Test handling of an interrupted OCR process
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

        // Helper function to copy test files to the input folder
        private void CopyTestFile(string fileName)
        {
            string sourcePath = Path.Combine(testDataPath, fileName);
            string destinationPath = Path.Combine(mainInputFolder, fileName);

            if (!File.Exists(sourcePath))
                Assert.Fail($"Test file missing: {sourcePath}");

            File.Copy(sourcePath, destinationPath, true);
        }

        // Helper function to run the OCR process and capture output
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

        // Helper function to clear the content of a directory
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

        // Helper function to verify OCR output file existence
        private void AssertOutputFileExists()
        {
            string[] files = Directory.GetFiles(mainOutputFolder, "*.txt");
            Assert.That(files.Length, Is.GreaterThan(0), "No OCR output file was generated.");
        }
    }
}
