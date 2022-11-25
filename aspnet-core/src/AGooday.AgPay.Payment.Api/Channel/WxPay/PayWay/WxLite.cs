using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.PayWay
{
    /// <summary>
    /// 微信 小程序支付
    /// </summary>
    public class WxLite : WxPayPaymentService
    {
        private readonly IPaymentService wxJsapi;

        public WxLite(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
            this.wxJsapi = serviceProvider.GetService<WxJsapi>(); //serviceProvider.GetServices<IPaymentService>().FirstOrDefault(f => f.GetType().Equals(typeof(WxJsapi)));
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return wxJsapi.Pay(rq, payOrder, mchAppConfigContext);
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return wxJsapi.PreCheck(rq, payOrder);
        }
    }
}
