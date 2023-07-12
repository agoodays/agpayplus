using AGooday.AgPay.Payment.Api.Channel.UmsPay.PayWay;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.Extensions
{
    public class UmsPayNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPaymentService, AliBar>();
        }
    }
}
