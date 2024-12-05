using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;

namespace AGooday.AgPay.Components.Third.Channel.WxPay.PayWayV3
{
    /// <summary>
    /// 微信 条码支付
    /// </summary>
    public class WxBar : WxPayPaymentService
    {
        private readonly IPaymentService wxBar;

        public WxBar(ILogger<WxBar> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            this.wxBar = serviceProvider.GetService<PayWay.WxBar>(); //serviceProvider.GetServices<IPaymentService>().FirstOrDefault(f => f.GetType().Equals(typeof(PayWay.WxBar)));
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
