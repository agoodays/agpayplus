using AGooday.AgPay.Payment.Api.Channel.UmsPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.Extensions
{
    public class UmsPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, AliBar>();
            services.AddSingleton<IPaymentService, AliJsapi>();
            services.AddSingleton<IPaymentService, AliQr>();
            services.AddSingleton<IPaymentService, WxBar>();
            services.AddSingleton<IPaymentService, WxJsapi>();
            services.AddSingleton<IPaymentService, YsfBar>();
            services.AddSingleton<IPaymentService, YsfJsapi>();
        }
    }
}
