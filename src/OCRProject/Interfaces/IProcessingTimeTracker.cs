using System;
using System.Collections.Generic;

namespace OCRProject.Interfaces;

    public interface IProcessingTimeTracker
    {
        void StartTimer();
        void StopAndRecord(string imageName, string processingStep);
        void GenerateExcelReport();
    }