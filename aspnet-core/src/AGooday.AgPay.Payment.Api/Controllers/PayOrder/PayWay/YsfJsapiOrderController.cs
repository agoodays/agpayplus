using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 云闪付 jsapi支付
    /// </summary>
    [ApiController]
    public class YsfJsapiOrderController : AbstractPayOrderController
    {
        public YsfJsapiOrderController(ILogger<YsfJsapiOrderController> logger,
            IChannelServiceFactory<IPaymentService> paymentServiceFactory,
            PayOrderProcessService payOrderProcessService,
            IMchPayPassageService mchPayPassageService,
            IPayRateConfigService payRateConfigService,
            IPayWayService payWayService,
            IPayOrderService payOrderService,
            IPayOrderProfitService payOrderProfitService,
            ISysConfigService sysConfigService,
            IMQSender mqSender,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, paymentServiceFactory, payOrderProcessService, mchPayPassageService, payRateConfigService, payWayService, payOrderService, payOrderProfitService, sysConfigService, mqSender, requestKit, configContextQueryService)
        {
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        [HttpPost, Route("api/pay/ysfJsapiOrder")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER)]
        public async Task<ActionResult<ApiRes>> YsfJsapiOrderAsync()
        {
            //获取参数 & 验证
            YsfJsapiOrderRQ bizRQ = await this.GetRQByWithMchSignAsync<YsfJsapiOrderRQ>();

            // 统一下单接口
            return await UnifiedOrderAsync(CS.PAY_WAY_CODE.YSF_JSAPI, bizRQ);
        }
    }
}
