using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWayV3
{
    /// <summary>
    /// 微信 条码支付
    /// </summary>
    public class WxBar : WxPayPaymentService
    {
        private readonly PayWay.WxBar wxBar;

        public WxBar(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService,
            PayWay.WxBar wxBar)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            this.wxBar = wxBar;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return wxBar.PreCheck(rq, payOrder);
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return wxBar.Pay(rq, payOrder, mchAppConfigContext);
        }
    }
}
