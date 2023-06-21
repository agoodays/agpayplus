using AGooday.AgPay.Payment.Api.Channel.LesPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay.Extensions
{
    public class LesPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, AliBar>();
            services.AddSingleton<IPaymentService, AliQr>();
            services.AddSingleton<IPaymentService, AliJsapi>();

            services.AddSingleton<IPaymentService, WxBar>();
            services.AddSingleton<IPaymentService, WxJsapi>();

            services.AddSingleton<IPaymentService, YsfBar>();
            services.AddSingleton<IPaymentService, YsfJsapi>();
        }
    }
}
