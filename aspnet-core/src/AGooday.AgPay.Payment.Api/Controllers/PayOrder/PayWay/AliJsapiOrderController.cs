using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 支付宝 jspai
    /// </summary>
    [ApiController]
    public class AliJsapiOrderController : AbstractPayOrderController
    {
        public AliJsapiOrderController(IMQSender mqSender,
            Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            RequestIpUtil requestIpUtil,
            ILogger<AliJsapiOrderController> logger,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
            : base(mqSender, paymentServiceFactory, configContextQueryService, payOrderProcessService, requestIpUtil, logger, mchPayPassageService, payOrderService, sysConfigService)
        {
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        [HttpPost]
        [Route("api/pay/aliJsapiOrder")]
        public ActionResult<ApiRes> AliJsapiOrder(AliJsapiOrderRQ bizRQ)
        {
            // 统一下单接口
            return UnifiedOrder(CS.PAY_WAY_CODE.ALI_JSAPI, bizRQ);// "ALI_JSAPI";  //支付宝服务窗支付
        }
    }
}
