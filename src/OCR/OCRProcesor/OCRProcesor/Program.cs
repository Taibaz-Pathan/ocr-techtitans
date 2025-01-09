using Patagames.Ocr;
using Patagames.Ocr.Enums;

namespace OCRProcesor
{
    class Program
    {
        static void Main()
        {
            using (var api = OcrApi.Create())
            {
                api.Init(Languages.English);
                string plainText = api.GetTextFromImage("/Users/khushalsingh/Downloads/test_02.jpg");
                Console.WriteLine(plainText);
            }
        }
    }
}