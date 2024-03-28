using AGooday.AgPay.Components.OCR.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AGooday.AgPay.Components.OCR.Services
{
    public abstract class AbstractOcrService : IOcrService
    {
        protected readonly ILogger<AbstractOcrService> _logger;

        protected AbstractOcrService(ILogger<AbstractOcrService> logger)
        {
            _logger = logger;
        }

        public abstract Task<CardOCRResult> RecognizeCardTextAsync(string imageUrl, string type);
        public abstract Task<string> RecognizeTextAsync(string imageUrl, string type);

        protected static string ConvertEmptyStringToNull(string input) => string.IsNullOrWhiteSpace(input) ? null : input;

        protected static string ConvertDateToFormat(string date, string inputFormat, string outputFormat = "yyyy-MM-dd") => DateTime.TryParseExact(date, inputFormat, null, DateTimeStyles.None, out DateTime dateTime) ? dateTime.ToString(outputFormat) : date;
    }
}
