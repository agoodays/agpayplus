using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付接口： 支付宝官方
    /// 支付方式： 自适应
    /// </summary>
    public class AliPayPaymentService : AbstractPaymentService
    {
        public AliPayPaymentService(ILogger<AliPayPaymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public AliPayPaymentService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override Task<AbstractRS> PayAsync(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PayAsync(bizRQ, payOrder, mchAppConfigContext);
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            return PayWayUtil.GetRealPayWayService(this, payOrder.WayCode).PreCheck(bizRQ, payOrder);
        }
    }
}
