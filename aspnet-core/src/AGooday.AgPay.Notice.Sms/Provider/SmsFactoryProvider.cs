using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AGooday.AgPay.Notice.Sms
{
    internal class SmsFactoryProvider : ISmsFactoryProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SmsOptions _smsOptions;

        public SmsFactoryProvider(IServiceProvider serviceProvider, IOptions<SmsOptions> smsOptions)
        {
            _serviceProvider = serviceProvider;
            _smsOptions = smsOptions.Value;
        }

        public ISmsProvider GetProvider()
        {
            return _smsOptions.SmsUseType switch
            {
                "aliyunSms" => _serviceProvider.GetService<AliyunSmsProvider>(),
                _ => throw new Exception($"Invalid provider: {_smsOptions.SmsUseType}"),
            };
        }
    }
}
