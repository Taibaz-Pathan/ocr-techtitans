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

## Installation
1. Prerequisites
Before using this project, ensure you have the following installed:
- .NET 9: Required to build and run the project.
- Tesseract OCR Engine: Ensure Tesseract is installed and accessible on your system. You can find installation instructions here.
- NuGet Packages: The required NuGet packages will be automatically installed when you restore the project.
  
2. Installation
Clone or download the project:
git clone https://github.com/Taibaz-Pathan/ocr-techtitans.git
After cloning the repository, navigate to the project folder and restore the necessary NuGet packages:
dotnet restore

## Usage Instructions
Once the project is set up, follow these steps to run the OCR pipeline:
1. Place Your Raw Images:Place the raw images that need to be processed in the Input/ folder. 
2. Install Tessdata from https://github.com/tesseract-ocr/tessdata
3. Add your Open AI API Key in Utils/mycode.json file. 
4. Build the Project: Open a command prompt or terminal and navigate to the project directory. Run the following command to build the project- dotnet build.   
5.Run the application: Navigate to the output directory:cd bin/Debug/net9.0/OCRProject.exe. This will process the input images, extract the text, and save the results in the Output folder.


## Project Structure
![Project Structure](./assets/ProjectFolderStructure.png)  
- `OCRProject.csproj`: Project configuration file.
- `Program.cs`: Main entry point of the application.
- `Input/`: Folder containing input image files.
- `Output/`: Folder for saving processed images and extracted text.
- `ModelComparison/`: Folder for storing model comparison results and embeddings.
- `ImageProcessing/`: Folder for image preprocessing scripts.
- `TesseractProcessor/`: Folder for handling text extraction via Tesseract.
- `Utils/`: Folder for utility scripts.

## Project Workflow
The OCR Project follows a pipeline for each input image through several steps:
1. Image Preprocessing
Input: Raw image (e.g png, jpg, jpeg etc). Raw images are to be store in Input Folder
Process: Different pre-processing techniques are applied to the input image to remove noise, remove blurring or image and obtain a clean image. Preprocessing class     are store in ImageProcessing folder.Following are the preprocessing techniques used:
- ConvertToGrayscale- Converts a color image to grayscale by reducing it to shades of gray, simplifying the data for better OCR accuracy.
- Deskew.cs- Corrects any tilt or rotation in the image, aligning the text horizontally to improve OCR results.
- GlobalThresholding.cs- Converts the grayscale image into a binary image (black and white) by applying a global threshold, improving contrast for text detection.
- SaturationAdjustment.cs- Adjusts the saturation of the image to enhance color intensity, which can help with clarity in certain image types.
- AdaptiveThreshold.cs- SConverts the image based on local threshold and variation in brightness


2. Text Extraction (OCR)
Input: Pre-processed image (obtained from the previous step-stored in folder Output/ProcessedImage).
Process: 
Utilize Tesseract OCR to extract text from the image.
Tesseract processes the image and generates raw text based on detected characters.
Output: Extracted text is stored in the text file by Name- ProcessedFile_yyyyMMdd_HHmmss.txt (example-ProcessedFile_20250323_154732). File is stored in Output/ ExtractedText. The extracted text is also stored in a dictionary.

3. Generate Embeddings
Input: A dictionary of extracted Texts, where keys are model names and values are the extracted texts.
Process: Random embeddings (vectors) for extracted texts is generated by EmbeddingGeneratorService.cs. The service is stored in ModelComparision folder.
Output: Structured output in the desired format, typically as text files, CSV, or JSON.

4. Cosine Similarity Calculation: The generated embeddings are compared using the cosine similarity method to measure the similarity between different preprocessing approaches. The results of cosine similarity matrix are stored in Output/Comparison folder by name CosineSimilarity.xlsx.

5. Time Tracking and memory usage- While we apply preprocessing technique and extract text using Tessearct we calculate the time taken by each processing step and the memory usage of that pre processing step. The results are the stored in Output/Comparison folder by name ProcessingResults.xlsx.

6. Ranking the preprocessing- Based on results of cosine similarity, time taken by pre-processing method and memory usage-each pre processing step is ranked from high to low order and is store in Output/Comparison folder by name BestModelRanking.xlsx

## Dependencies
Required Libraries
The following libraries are required to run the OCR project. You can install these dependencies via NuGet.

For OCR Processing:
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using Tesseract;

For Image Preprocessing:
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing; 
using System.Linq;

For Configuration & Utilities:
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

For Cosine similarity and Conversion to matrix:
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OCRProject.Interfaces;
using System.Diagnostics;

## Unit test Project
https://github.com/Taibaz-Pathan/ocr-techtitans/tree/main/src/OCRTestProject

Team Members
* Mithila Prabhu
* Taibaz Pathan
* Khushal Singh
