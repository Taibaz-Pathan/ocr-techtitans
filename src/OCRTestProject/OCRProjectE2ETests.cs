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
        private readonly string testDataPath = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRTestProject\TestData";
        private readonly string executablePath = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRProject\bin\Debug\net9.0-windows\OCRProject.exe";

        private readonly string testInputFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRTestProject\input";
        private readonly string testOutputFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRTestProject\output";
        private readonly string logFile = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRTestProject\logs\log.txt";

        private readonly string mainInputFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRProject\Input";
        private readonly string mainOutputFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRProject\Output\ExtractedText";
        private readonly string backupFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRProject\Input_backup";

        [SetUp]
        public void Setup()
        {
            if (!File.Exists(executablePath))
            {
                Assert.Fail($"Executable not found: {executablePath}");
            }

            Directory.CreateDirectory(testInputFolder);
            Directory.CreateDirectory(testOutputFolder);
            ClearDirectory(testInputFolder);
            ClearDirectory(testOutputFolder);

            BackupAndReplaceMainInputFolder();

            if (File.Exists(logFile)) File.WriteAllText(logFile, string.Empty);
        }

        [TearDown]
        public void Cleanup()
        {
            RestoreOriginalInputFolder();
        }

        [Test]
        public void Test_OCR_Empty_Input_Folder()
        {
            string output = RunOCRProcess();

            Assert.That(output, Does.Contain("No images found"), "OCR process did not detect an empty input folder correctly.");
            Assert.That(File.Exists(logFile), "Log file was not created.");
        }

        [Test]
        public void Test_OCR_Processing_Valid_Image()
        {
            CopyTestFile("sample-image.png");
            string output = RunOCRProcess();

            Assert.That(output, Does.Contain("Processing completed"), "OCR did not complete successfully.");
            AssertOutputFileExists();
        }

        [Test]
        public void Test_OCR_Processing_Invalid_File()
        {
            CopyTestFile("test.txt");
            string output = RunOCRProcess();

            Assert.That(output, Does.Contain("Unsupported file format").Or.Contain("No images found"),
                        "OCR did not handle invalid file format correctly.");
            Assert.That(File.Exists(logFile), "Error log file was not created.");
        }

        [Test]
        public void Test_OCR_Processing_Multiple_Valid_Images()
        {
            CopyTestFile("sample-image1.png");
            CopyTestFile("sample-image2.png");
            string output = RunOCRProcess();

            Assert.That(output, Does.Contain("Processing completed"), "OCR did not complete successfully.");
            AssertOutputFileExists(2);
        }        

        [Test]
        public void Test_OCR_Empty_Output_File()
        {
            CopyTestFile("blank-image.png"); // An image with no text
            string output = RunOCRProcess();

            string[] outputFiles = Directory.GetFiles(mainOutputFolder, "*.txt");
            Assert.That(outputFiles.Length, Is.GreaterThan(0), "No output file was generated.");
            string text = File.ReadAllText(outputFiles[0]);
            Assert.That(text.Trim(), Is.Empty, "OCR output file should be empty for a blank image.");
        }

        private void CopyTestFile(string fileName)
        {
            string sourcePath = Path.Combine(testDataPath, fileName);
            string destinationPath = Path.Combine(mainInputFolder, fileName);

            if (!File.Exists(sourcePath))
            {
                Assert.Fail($"Test file missing: {sourcePath}");
            }

            File.Copy(sourcePath, destinationPath, true);
        }

        private string RunOCRProcess()
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = executablePath;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(executablePath);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                string output = "";
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        Console.WriteLine($"[OCR Output] {e.Data}");
                        output += e.Data + Environment.NewLine;
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        Console.WriteLine($"[OCR Error] {e.Data}");
                        output += "[ERROR] " + e.Data + Environment.NewLine;
                    }
                };

                try
                {
                    Console.WriteLine($"Starting OCR Process: {executablePath}");
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    bool exited = process.WaitForExit(60000);

                    if (!exited)
                    {
                        process.Kill();
                        throw new Exception("OCR process timed out.");
                    }

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"OCR process failed with exit code {process.ExitCode}");
                    }

                    return output;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to run OCR process: {ex.Message}");
                }
            }
        }

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

        private void AssertOutputFileExists(int expectedFileCount = 1)
        {
            string[] files = Directory.GetFiles(mainOutputFolder, "*.txt");
            Assert.That(files.Length, Is.EqualTo(1),
                        $"Expected {expectedFileCount} OCR output files, but found {files.Length}.");
            foreach (var file in files)
            {
                string text = File.ReadAllText(file);
                Assert.That(text, Is.Not.Empty, "OCR output text is empty.");
            }
        }

        private void BackupAndReplaceMainInputFolder()
        {
            if (Directory.Exists(mainInputFolder))
            {
                if (Directory.Exists(backupFolder))
                    Directory.Delete(backupFolder, true);
                Directory.Move(mainInputFolder, backupFolder);
            }

            Directory.CreateDirectory(mainInputFolder);
            foreach (string file in Directory.GetFiles(testInputFolder))
            {
                string destFile = Path.Combine(mainInputFolder, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
        }

        private void RestoreOriginalInputFolder()
        {
            if (Directory.Exists(mainInputFolder))
                Directory.Delete(mainInputFolder, true);

            if (Directory.Exists(backupFolder))
                Directory.Move(backupFolder, mainInputFolder);
        }
    }
}
