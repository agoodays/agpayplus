using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OCR.Constants;
using AGooday.AgPay.Components.OCR.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Components.OCR.Extensions
{
    public class OCRNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddKeyedScoped<IOcrService, AliyunOcrService>(OcrTypeEnum.Aliyun);
            services.AddKeyedScoped<IOcrService, TencentOcrService>(OcrTypeEnum.Tencent);
            services.AddScoped<IOcrServiceFactory, OcrServiceFactory>();
        }
    }

    public interface IOcrServiceFactory
    {
        IOcrService GetService();
    }

    public class OcrServiceFactory : IOcrServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISysConfigService _sysConfigService;

        public OcrServiceFactory(IServiceProvider serviceProvider, ISysConfigService sysConfigService)
        {
            _serviceProvider = serviceProvider;
            _sysConfigService = sysConfigService;
        }

        public IOcrService GetService()
        {
            var ocrType = GetOcrType();
            return _serviceProvider.GetRequiredKeyedService<IOcrService>(ocrType);
        }

        private OcrTypeEnum GetOcrType()
        {
            return (OcrTypeEnum)_sysConfigService.GetDBOcrConfig().OcrType;
        }
    }
}
