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
        private readonly IPaymentService _paymentService;

        public WxBar(ILogger<WxBar> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
            _paymentService = serviceProvider.GetService<PayWay.WxBar>(); //serviceProvider.GetServices<IPaymentService>().FirstOrDefault(f => f.GetType().Equals(typeof(PayWay.WxBar)));
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return _paymentService.PreCheckAsync(rq, payOrder, mchAppConfigContext);
        }

        public override Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return _paymentService.PayAsync(rq, payOrder, mchAppConfigContext);
        }
    }
}
