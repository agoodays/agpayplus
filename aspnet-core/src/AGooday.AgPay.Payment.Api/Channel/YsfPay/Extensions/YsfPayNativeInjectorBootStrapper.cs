using AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.Extensions
{
    public class YsfPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, AliBar>();
            services.AddSingleton<IPaymentService, AliJsapi>();

            services.AddSingleton<IPaymentService, WxBar>();
            services.AddSingleton<IPaymentService, WxJsapi>();

            services.AddSingleton<IPaymentService, YsfBar>();
            services.AddSingleton<IPaymentService, YsfJsapi>();
        }
    }
}
