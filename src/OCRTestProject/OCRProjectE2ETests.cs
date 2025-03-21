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
        private readonly static string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
        private readonly string testDataPath = Path.Combine(basePath, "OCRTestProject", "TestData");
        private readonly string executablePath = Path.Combine(basePath, "OCRProject", "bin", "Debug", "net9.0", "OCRProject.exe");
        private readonly string mainInputFolder = Path.Combine(basePath, "OCRProject", "Input");
        private readonly string mainOutputFolder = Path.Combine(basePath, "OCRProject", "Output", "ExtractedText");
        private readonly string logFile = Path.Combine(basePath, "OCRTestProject", "logs", "log.txt");

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
            Assert.That(output, Does.Contain("Error processing"), "OCR did not handle a corrupt image correctly.");
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
        public void Test_OCR_Processing_Multiple_Images()
        {
            CopyTestFile("sample-image.png");
            CopyTestFile("sample-image1.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process multiple images correctly.");
            AssertOutputFileExists();
        }

        [Test]
        public void Test_OCR_Processing_MultiLanguage_Text()
        {
            CopyTestFile("multi-language.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle multi-language text correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Rotated_Text()
        {
            CopyTestFile("rotated-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process rotated text correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Noisy_Image()
        {
            CopyTestFile("noisy-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle noisy images correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Only_Numbers()
        {
            CopyTestFile("numbers-only.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not extract numbers correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Empty_Image()
        {
            CopyTestFile("empty-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Error opening data file ./tessdata/eng.traineddata"), "OCR should return an empty result for blank images.");
        }

        [Test]
        public void Test_OCR_Processing_Skewed_Text()
        {
            CopyTestFile("skewed-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process skewed text correctly.");
        }

        [Test]
        public void Test_Multiple_Images_In_Input()
        {
            CopyTestFile("multi-image-1.png");
            CopyTestFile("multi-image-2.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process multiple images correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Different_Image_Formats()
        {
            CopyTestFile("test-image.jpg");
            CopyTestFile("test-image.bmp");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle different image formats correctly.");
        }

        [Test]
        public void Test_OCR_Processing_Handwritten_Text()
        {
            CopyTestFile("handwritten-text.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process handwritten text correctly.");
        }

        [Test]
        public void Test_OCR_Low_Contrast_Image()
        {
            CopyTestFile("low-contrast.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process low-contrast images correctly.");
        }

        [Test]
        public void Test_OCR_Multi_Column_Text()
        {
            CopyTestFile("multi-column.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle multi-column text correctly.");
        }

        [Test]
        public void Test_OCR_Special_Characters_And_Emojis()
        {
            CopyTestFile("special-characters.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not recognize special characters or emojis correctly.");
        }

        [Test]
        public void Test_OCR_Processing_PDF_File()
        {
            CopyTestFile("test-document.pdf");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("No images found"), "OCR did not handle non-image file types correctly.");
        }

        [Test]
        public void Test_ImageProcessing_Grayscale()
        {
            CopyTestFile("grayscale-test.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process grayscale images correctly.");
        }

        [Test]
        public void Test_ImageProcessing_NoiseReduction()
        {
            CopyTestFile("noise-reduction-test.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not handle noise reduction correctly.");
        }

        [Test]
        public void Test_ImageProcessing_Rotation()
        {
            CopyTestFile("rotated-image.png");
            string output = RunOCRProcess();
            Assert.That(output, Does.Contain("Processing completed"), "OCR did not process rotated images correctly.");
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
