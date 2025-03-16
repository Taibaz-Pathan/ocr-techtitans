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
        private readonly string mainInputFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRProject\Input";
        private readonly string mainOutputFolder = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRProject\Output\ExtractedText";
        private readonly string logFile = @"C:\Users\mithi\OneDrive\Desktop\SoftwareEngg\ocr-techtitans\src\OCRTestProject\logs\log.txt";

        [SetUp]
        public void Setup()
        {
            if (!File.Exists(executablePath))
                Assert.Fail($"Executable not found: {executablePath}");

            Directory.CreateDirectory(mainInputFolder);
            Directory.CreateDirectory(mainOutputFolder);
            ClearDirectory(mainInputFolder);
            ClearDirectory(mainOutputFolder);

            if (File.Exists(logFile)) File.WriteAllText(logFile, string.Empty);
        }

        [Test]
        public void Test_OCR_Empty_Input_Folder()
        {
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found"), "OCR did not detect an empty input folder correctly.");
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
        public void Test_OCR_Processing_Corrupt_Image()
        {
            CopyTestFile("corrupt-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Error processing file"), "OCR did not handle a corrupt image correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Large_Image()
        {
            CopyTestFile("large-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle large images correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Invalid_File()
        {
            CopyTestFile("test.txt");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found in the input folder."), "OCR did not handle invalid file format correctly.");
        }        

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

        private void CopyTestFile(string fileName)
        {
            string sourcePath = Path.Combine(testDataPath, fileName);
            string destinationPath = Path.Combine(mainInputFolder, fileName);

            if (!File.Exists(sourcePath))
                Assert.Fail($"Test file missing: {sourcePath}");

            File.Copy(sourcePath, destinationPath, true);
        }

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

        private void AssertOutputFileExists()
        {
            string[] files = Directory.GetFiles(mainOutputFolder, "*.txt");
            Assert.That(files.Length, Is.GreaterThan(0), "No OCR output file was generated.");
        }
    }
}
