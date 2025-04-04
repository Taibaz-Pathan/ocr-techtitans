# OCRProject

## Overview
The OCR (Optical Character Recognition) Project is designed to process images, extract meaningful text data, and perform image preprocessing for better OCR accuracy. This project leverages Tesseract for optical character recognition and includes a modular pipeline for processing images, extracting text, and organizing output data into structured formats. The solution is built in .NET and provides flexibility for OCR tasks such as extracting text from various document formats.

## Features
- Reads image files
- Uses OCR to extract text
- Outputs recognized text to  file
- Generates Embeddings for extracted Text
- Calculates cosine similarity
- Tracks time taken by each processing step
- Tracks memory usage
- Computes the best processing model based on cosine similarity, time taken by processing step and memory usage
- Captures all the logging trace in Text file.

## Prerequisites
- .NET SDK (latest version recommended)
- Visual Studio or any C# IDE
- Tesseract OCR library 
- OpenAI library
- Tessdata

## Installation and Usage Instructions
Step 1- Clone the project from git using git clone command, Link clone Project-https://github.com/Taibaz-Pathan/ocr-techtitans.git <br>    
![clone](./assets/step1.1.png) <br>
Figure1. Git Clone of Project<br>

![clone](./assets/step1.2.png)<br>
Figure2. Cmd image of Project getting cloned<br>

Step 2- Go to project folder. Use command cd OCRProject
![Folder](./assets/step2.2.png)<br>
Figure3. Navigate to project folder

Step 3- Install dependencies. Use dotnet restore to install dependencies. OCRProject.csproj file already mentions the required dependencies to be installed<br>
![Install](./assets/step3.png)<br>
Figure4. Installation step<br>

Step 4-Build the project after the dependencies are installed. Use dotnet build command. <br>
![build](./assets/step4.png)<br>
Figure5. Build Project <br>

Step 5- Open Input folder to place your input-images. The project already has few images. \ocr-techtitans\OCRProject\Input<br>
![img](./assets/step5.png)<br>
Figure6. Input Image folder<br>

Step 6- Go to Utils folder- \ocr-techtitans\OCRProject\Utils<br>
![util](./assets/step6.png)<br>
Figure7. Utils folder<br>

Step 7- Open mycode.json and place your open ai chatgpt key <br>
Replace “YOUR-API-KEY” with actual api key<br>
![key](./assets/step7.png)<br>
Figure8. Replace with your key<br>

Step 8- To open the project code, import the working directory in visual studio or click on OCRProject.sln<br>
![import](./assets/step8.png)<br>
Figure9. Replace with your key<br>

Step 9- To run the project click on Green arrow in Visual Studio<br>
![run](./assets/step9.1.png)<br>
Figure10. Visual Studio button<br>

or Go to \OCRProject\bin\Debug\net9.0 and execute/double click on OCRProject.exe <br>
![run](./assets/step9.2.png)<br>
Figure11. OCRProject.exe file <br>

You will see these message once you run<br>
![run](./assets/step9.3.png)<br>
Figure12. Code getting executed

Once the code is executed, you will see below message<br>
![run](./assets/step9.4.png)<br>
Figure13. Code execution completion message

Step 10- Check the output folder for the results. <br>
Folder OCRProject\Output\Comparision- Contains 3 excel files.<br>
BestModelRanking.xlsx – contains the models ranked from best to worstCosineSimilarity.xlsx- Contains the cosine matrix generated<br>
ProcessingResults.xlsx- Contains time taken by each preprocessing step and memory usage metrics<br>
Folder OCRProject\Output\ExtractedText- Contains one text file with all the extracted texts<br>
Folder OCRProject\Output\ Logs- Contains one text file which ia alog file that captures any warning/error message. File name is AppLog.txt <br>
Folder \OCRProject\Output\ProcessedImage- Contains all the images generated after applying preprocessing <br>

## Project Structure
![Project Structure](./assets/ProjectFolderStructure.png)
Figure14. Project File Structure
- `OCRProject.csproj`: Project configuration file.
- `Program.cs`: Main entry point of the application.
- `Input/`: Folder containing input image files.
- `Output/`: Folder for saving processed images and extracted text.
- `ModelComparison/`: Folder for storing model comparison results and embeddings.
- `ImageProcessing/`: Folder for image preprocessing scripts.
- `TesseractProcessor/`: Folder for handling text extraction via Tesseract.
- `Utils/`: Folder for utility scripts.

## Project Workflow
The OCR Project follows a pipeline for each input image through several steps:<br>
1. Image Preprocessing<br>
Input: Raw image (e.g png, jpg, jpeg etc). Raw images are to be store in Input Folder<br>
Process: Different pre-processing techniques are applied to the input image to remove noise, remove blurring or image and obtain a clean image. Preprocessing class     are store in ImageProcessing folder.Following are the preprocessing techniques used:<br>
- ConvertToGrayscale- Converts a color image to grayscale by reducing it to shades of gray, simplifying the data for better OCR accuracy.<br>
- Deskew.cs- Corrects any tilt or rotation in the image, aligning the text horizontally to improve OCR results.<br>
- GlobalThresholding.cs- Converts the grayscale image into a binary image (black and white) by applying a global threshold, improving contrast for text detection.<br>
- SaturationAdjustment.cs- Adjusts the saturation of the image to enhance color intensity, which can help with clarity in certain image types.<br>
- AdaptiveThreshold.cs- SConverts the image based on local threshold and variation in brightness<br>


2. Text Extraction (OCR)
Input: Pre-processed image (obtained from the previous step-stored in folder Output/ProcessedImage).<br>
Process: <br>
Utilize Tesseract OCR to extract text from the image.<br>
Tesseract processes the image and generates raw text based on detected characters.<br>
Output: Extracted text is stored in the text file by Name- ProcessedFile_yyyyMMdd_HHmmss.txt (example-ProcessedFile_20250323_154732). File is stored in Output/ ExtractedText. The extracted text is also stored in a dictionary.

3. Generate Embeddings<br>
Input: A dictionary of extracted Texts, where keys are model names and values are the extracted texts.<br>
Process: Random embeddings (vectors) for extracted texts is generated by EmbeddingGeneratorService.cs. The service is stored in ModelComparision folder.<br>
Output: Structured output in the desired format, typically as text files, CSV, or JSON.<br>

4. Cosine Similarity Calculation: The generated embeddings are compared using the cosine similarity method to measure the similarity between different preprocessing approaches. The results of cosine similarity matrix are stored in Output/Comparison folder by name CosineSimilarity.xlsx.<br>

5. Time Tracking and memory usage- While we apply preprocessing technique and extract text using Tessearct we calculate the time taken by each processing step and the memory usage of that pre processing step. The results are the stored in Output/Comparison folder by name ProcessingResults.xlsx.<br>

6. Ranking the preprocessing- Based on results of cosine similarity, time taken by pre-processing method and memory usage-each pre processing step is ranked from high to low order and is store in Output/Comparison folder by name BestModelRanking.xlsx<br>

## Dependencies
Required Libraries<br>
The following libraries are required to run the OCR project. You can install these dependencies via NuGet.<br>

For OCR Processing:<br>
using System;<br>
using System.IO;<br>
using SixLabors.ImageSharp;<br>
using SixLabors.ImageSharp.PixelFormats;<br>
using SixLabors.ImageSharp.Processing;<br>
using SixLabors.ImageSharp.Formats.Png;<br>
using Tesseract;<br>

For Image Preprocessing:<br>
using System;<br>
using SixLabors.ImageSharp;<br>
using SixLabors.ImageSharp.PixelFormats;<br>
using SixLabors.ImageSharp.Processing; <br>
using System.Linq;<br>

For Configuration & Utilities:<br>
using System;<br>
using System.IO;<br>
using System.Reflection;<br>
using System.Collections.Generic;<br>
using System.Linq;<br>
using System.Text;<br>
using System.Threading.Tasks;<br>

For Cosine similarity and Conversion to matrix:<br>
using System;<br>
using System.Collections.Generic;<br>
using System.IO;<br>
using System.Linq;<br>
using NPOI.SS.UserModel;<br>
using NPOI.XSSF.UserModel;<br>
using OCRProject.Interfaces;<br>
using System.Diagnostics;<br>

## Unit test Project
https://github.com/Taibaz-Pathan/ocr-techtitans/tree/main/src/OCRTestProject<br>

## Steps to run Unit test 
Step 1- Import the UnitTest Project in Visual Studio. OCRProjectE2ETests.cs contains all the end to end unit tests<br>
![Unit Test](./assets/Unit1.png)<br>
Figure15. Unit test Folder<br>

Step 2- To run the unit tests, go to Tests tab and click on Run All Tests<br>
![Unit Test](./assets/Unit2.png)<br>
Figure16. Run tests

Step 3- After running the test, you will see if the tests failed or passed<br>
![Unit Test](./assets/UnitTest.png)<br>
Figure17. Unit test results<br>