using AGooday.AgPay.Payment.Api.Channel.LesPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay.Extensions
{
    public class LesPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPaymentService, AliBar>();
            services.AddScoped<IPaymentService, AliQr>();
            services.AddScoped<IPaymentService, AliJsapi>();

            services.AddScoped<IPaymentService, WxBar>();
            services.AddScoped<IPaymentService, WxJsapi>();

            services.AddScoped<IPaymentService, YsfBar>();
            services.AddScoped<IPaymentService, YsfJsapi>();
        }
    }
}
