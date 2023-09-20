using AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.Extensions
{
    public class AliPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPaymentService, AliApp>();
            services.AddScoped<IPaymentService, AliBar>();
            services.AddScoped<IPaymentService, AliJsapi>();
            services.AddScoped<IPaymentService, AliPc>();
            services.AddScoped<IPaymentService, AliQr>();
            services.AddScoped<IPaymentService, AliWap>();
        }
    }
}
