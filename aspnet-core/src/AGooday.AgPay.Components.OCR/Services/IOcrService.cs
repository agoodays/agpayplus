namespace AGooday.AgPay.Components.OCR.Services
{
    public interface IOcrService
    {
        Task<string> RecognizeTextAsync(string imagePath, string type);
    }
}
