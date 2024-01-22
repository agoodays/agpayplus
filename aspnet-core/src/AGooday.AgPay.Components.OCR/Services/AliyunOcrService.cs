using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OCR.Models;
using Microsoft.Extensions.Logging;

namespace AGooday.AgPay.Components.OCR.Services
{
    public class AliyunOcrService : IOcrService
    {
        private readonly ILogger<AliyunOcrService> logger;
        private readonly AliyunOcrConfig ocrConfig;

        public AliyunOcrService(ILogger<AliyunOcrService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
            var dbOcrConfig = sysConfigService.GetDBOcrConfig();
            ocrConfig = (AliyunOcrConfig)AbstractOcrConfig.GetOcrConfig(dbOcrConfig.OcrType, dbOcrConfig.AliOcrConfig);
        }

        public Task<string> RecognizeTextAsync(string imagePath, string type)
        {
            throw new NotImplementedException();
        }
    }
}
