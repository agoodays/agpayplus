using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 支付接口抽象类
    /// </summary>
    public abstract class AbstractPaymentService : IPaymentService
    {
        protected readonly ISysConfigService _sysConfigService;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ConfigContextQueryService _configContextQueryService;
        protected AbstractPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
        {
            _serviceProvider = serviceProvider;
            _configContextQueryService = configContextQueryService;
            _sysConfigService = sysConfigService;
        }

        public abstract string GetIfCode();
        public abstract bool IsSupport(string wayCode);
        public abstract AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext);
        public abstract string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder);

        public virtual long CalculateFeeAmount(long amount, decimal rate)
        {
            return AmountUtil.CalPercentageFee(amount, rate);
        }

        public virtual long CalculateProfitAmount(long amount, decimal rate)
        {
            return AmountUtil.CalPercentageFee(amount, rate);
        }

        /// <summary>
        /// 订单分账（一般用作 如微信订单将在下单处做标记）
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        protected bool IsDivisionOrder(PayOrderDto payOrder)
        {
            //订单分账， 将冻结商户资金。
            if (payOrder.DivisionMode != null && ((byte)PayOrderDivisionMode.DIVISION_MODE_AUTO == payOrder.DivisionMode || (byte)PayOrderDivisionMode.DIVISION_MODE_MANUAL == payOrder.DivisionMode))
            {
                return true;
            }
            return false;
        }

        protected string GetNotifyUrl()
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/pay/notify/{GetIfCode()}";
        }

        protected string GetNotifyUrl(string payOrderId)
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/pay/notify/{GetIfCode()}/{payOrderId}";
        }

        protected string GetReturnUrl()
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/pay/return/{GetIfCode()}";
        }

        protected string GetReturnUrl(string payOrderId)
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/pay/return/{GetIfCode()}/{payOrderId}";
        }
    }
}
