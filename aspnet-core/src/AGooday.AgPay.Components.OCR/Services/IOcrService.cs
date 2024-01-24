namespace AGooday.AgPay.Components.OCR.Services
{
    public interface IOcrService
    {
        Task<Dictionary<string, string>> RecognizeCardTextAsync(string imagePath, string type);
    }
}
