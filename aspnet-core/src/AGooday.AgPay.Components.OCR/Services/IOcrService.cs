namespace AGooday.AgPay.Components.OCR.Services
{
    public interface IOcrService
    {
        Task<string> RecognizeTextAsync(string imagePath, string type);
        Task<Dictionary<string, string>> RecognizeCardTextAsync(string imagePath, string type);
    }
}
