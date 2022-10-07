using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;

namespace AGooday.AgPay.Payment.Api.Channel
{
    public abstract class AbstractPaymentService : IPaymentService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected AbstractPaymentService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public abstract string GetIfCode();
        public abstract bool IsSupport(string wayCode);
        public abstract AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);
        public abstract string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder);
    }
}
