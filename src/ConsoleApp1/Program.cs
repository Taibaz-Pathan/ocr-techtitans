using Patagames.Ocr;
using Patagames.Ocr.Enums;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            using (var api = OcrApi.Create())
            {
                api.Init(Languages.English);
                string plainText = api.GetTextFromImage("C:/Users/mithi/OneDrive/Pictures/test_02.jpg");
                Console.WriteLine(plainText);
            }
        }
    }
}