using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel
{
    public abstract class AbstractRefundService : IRefundService
    {
        protected readonly ISysConfigService _sysConfigService;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ConfigContextQueryService _configContextQueryService;
        protected AbstractRefundService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
        {
            _serviceProvider = serviceProvider;
            _configContextQueryService = configContextQueryService;
            _sysConfigService = sysConfigService;
        }

        public abstract string GetIfCode();
        public abstract string PreCheck(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder);

        public virtual long CalculateFeeAmount(long amount, PayOrderDto payOrder)
        {
            var refundState = payOrder.RefundAmount + amount >= payOrder.Amount ? PayOrderRefund.REFUND_STATE_ALL : PayOrderRefund.REFUND_STATE_SUB;
            if (refundState.Equals(PayOrderRefund.REFUND_STATE_ALL))
            {
                return payOrder.MchFeeAmount;
            }
            return AmountUtil.CalPercentageFee(amount, payOrder.MchFeeRate);
        }

        public abstract ChannelRetMsg Query(RefundOrderDto refundOrder, MchAppConfigContext mchAppConfigContext);
        public abstract ChannelRetMsg Refund(RefundOrderRQ bizRQ, RefundOrderDto refundOrder, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);

        protected string GetNotifyUrl()
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/refund/notify/{GetIfCode()}";
        }

        protected string GetNotifyUrl(string refundOrderId)
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/refund/notify/{GetIfCode()}/{refundOrderId}";
        }
    }
}
