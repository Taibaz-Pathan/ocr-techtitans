public class TextFormatting
{
    public static string CleanOCRText(string inputText)
    {
        // Remove multiple spaces and replace with a single space
        string cleanedText = System.Text.RegularExpressions.Regex.Replace(inputText, @"\s+", " ");
        
        // Remove leading and trailing spaces
        cleanedText = cleanedText.Trim();

        // Optionally: Remove newlines or replace with a single space
        cleanedText = cleanedText.Replace("\n", " ").Replace("\r", " ");
        
        return cleanedText;
    }
}