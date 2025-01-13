using System;
using IronOcr;
class Program
{
    static void Main(string[] args)
    {
        // Set the license key for IronOCR (replace with your actual key)
        IronOcr.License.LicenseKey = "IRONSUITE.SINGHKHUSHAL925.GMAIL.COM.7593-2E8BA86780-AR4PBQI2VDARZC36-3KARAYRQDPOQ-QD45BNVBMENU-UP3ETUEFOFGY-PFME6ORWJ4UE-U7ZTJPIOJYTZ-QXARUJ-TSAOCDFUHF6OUA-DEPLOYMENT.TRIAL-PPXHI4.TRIAL.EXPIRES.10.FEB.2025";;

        // Initialize the OCR engine
        var ocr = new IronTesseract();

        try
        {
            // Create an OCR input object
            using (var ocrInput = new OcrInput())
            {
                // Load the image for OCR
                string imagePath = "/Users/khushalsingh/Downloads/test_02.jpg";
                ocrInput.LoadImage(imagePath);

                // Perform OCR and retrieve the result
                var ocrResult = ocr.Read(ocrInput);

                // Display the extracted text
                Console.WriteLine("OCR Result:");
                Console.WriteLine(ocrResult.Text);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and provide feedback
            Console.WriteLine("An error occurred during OCR processing:");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Optional: Clean up resources or log information
            Console.WriteLine("OCR processing completed.");
        }
    }
}
