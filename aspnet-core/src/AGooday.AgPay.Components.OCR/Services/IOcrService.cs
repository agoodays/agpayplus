using AGooday.AgPay.Components.OCR.Models;

namespace AGooday.AgPay.Components.OCR.Services
{
    public interface IOcrService
    {
        Task<string> RecognizeTextAsync(string imageUrl, string type);
        Task<CardOCRResult> RecognizeCardTextAsync(string imageUrl, string type);
    }
}
