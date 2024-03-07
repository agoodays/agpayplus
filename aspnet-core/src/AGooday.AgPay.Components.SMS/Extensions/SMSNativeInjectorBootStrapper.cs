using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.SMS.Constants;
using AGooday.AgPay.Components.SMS.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Components.SMS.Extensions
{
    public class SMSNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddKeyedScoped<AliyundySmsService>(SmsProviderCS.ALIYUNDY);
            services.AddScoped<ISmsServiceFactory, SmsServiceFactory>();
        }
    }

    public interface ISmsServiceFactory
    {
        ISmsService GetService();
        string GetSmsProviderKey();
    }

    public class SmsServiceFactory : ISmsServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISysConfigService _sysConfigService;

        public SmsServiceFactory(IServiceProvider serviceProvider, ISysConfigService sysConfigService)
        {
            _serviceProvider = serviceProvider;
            _sysConfigService = sysConfigService;
        }

        public ISmsService GetService()
        {
            var smsProviderKey = GetSmsProviderKey();
            return _serviceProvider.GetRequiredKeyedService<ISmsService>(smsProviderKey);
        }

        public string GetSmsProviderKey()
        {
            return _sysConfigService.GetDBSmsConfig().SmsProviderKey;
        }
    }
}
