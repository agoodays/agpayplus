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

        protected static async Task<byte[]> GetImageBytesAsync(string imageUrl)
        {
            try
            {
                if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(imageUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadAsByteArrayAsync();
                        }
                        else
                        {
                            // Handle unsuccessful response
                            return null;
                        }
                    }
                }
                else
                {
                    return await File.ReadAllBytesAsync(imageUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get Image Bytes Error: {ex.Message}");
                return null;
            }
        }

        protected static string ConvertEmptyStringToNull(string input) => string.IsNullOrWhiteSpace(input) ? null : input;

        protected static string ConvertDateToFormat(string date, string inputFormat, string outputFormat = "yyyy-MM-dd") => DateTime.TryParseExact(date, inputFormat, null, DateTimeStyles.None, out DateTime dateTime) ? dateTime.ToString(outputFormat) : date;
    }
}
