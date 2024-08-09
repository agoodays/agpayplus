using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.Services;

namespace AGooday.AgPay.Components.Third.Channel
{
    public abstract class AbstractRefundService : IRefundService
    {
        protected readonly ILogger<AbstractRefundService> _logger;
        protected readonly ISysConfigService _sysConfigService;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ConfigContextQueryService _configContextQueryService;
        protected AbstractRefundService(ILogger<AbstractRefundService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
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
