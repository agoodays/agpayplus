using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Notice.Sms
{
    internal class SmsOptionsExtension : INoticeOptionsExtension
    {
        private readonly Action<SmsOptions> configure;

        public SmsOptionsExtension(Action<SmsOptions> configure)
        {
            this.configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddOptions<SmsOptions>().Configure(configure);
            services.AddScoped<AliyunSmsProvider>();
            services.AddTransient<ISmsFactoryProvider, SmsFactoryProvider>();
        }
    }
}
