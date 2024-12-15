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
    /// 支付宝 条码支付
    /// </summary>
    [ApiController]
    public class AliBarOrderController : AbstractPayOrderController
    {
        public AliBarOrderController(ILogger<AliBarOrderController> logger,
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
        /// <returns></returns>
        [HttpPost, Route("api/pay/aliBarOrder")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER)]
        public async Task<ActionResult<ApiRes>> AliBarOrderAsync()
        {
            //获取参数 & 验证
            AliBarOrderRQ bizRQ = await this.GetRQByWithMchSignAsync<AliBarOrderRQ>();

            // 统一下单接口;
            return await UnifiedOrderAsync(CS.PAY_WAY_CODE.ALI_BAR, bizRQ);// "ALI_BAR";  //支付宝条码支付
        }
    }
}
