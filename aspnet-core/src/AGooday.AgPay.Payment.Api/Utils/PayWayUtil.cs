using AGooday.AgPay.Payment.Api.Channel;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Utils
{
    public class PayWayUtil
    {
        /** 获取真实的支付方式Service **/
        public static IPaymentService GetRealPaywayService(IServiceProvider serviceProvider, string wayCode)
        {
            try
            {
                IPaymentService paymentService = serviceProvider.GetServices<IPaymentService>().FirstOrDefault(f => f.GetType().Name.Equals(wayCode.Replace("_", ""), StringComparison.OrdinalIgnoreCase));
                return paymentService;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
