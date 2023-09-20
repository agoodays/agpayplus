using AGooday.AgPay.Payment.Api.Channel.UmsPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.Extensions
{
    public class UmsPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPaymentService, AliBar>();
            services.AddScoped<IPaymentService, AliJsapi>();
            services.AddScoped<IPaymentService, AliQr>();
            services.AddScoped<IPaymentService, WxBar>();
            services.AddScoped<IPaymentService, WxJsapi>();
            services.AddScoped<IPaymentService, YsfBar>();
            services.AddScoped<IPaymentService, YsfJsapi>();
        }
    }
}
