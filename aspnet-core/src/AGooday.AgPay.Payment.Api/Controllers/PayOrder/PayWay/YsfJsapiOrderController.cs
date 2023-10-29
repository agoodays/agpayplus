using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Authorization;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 云闪付 jsapi支付
    /// </summary>
    [ApiController]
    public class YsfJsapiOrderController : AbstractPayOrderController
    {
        public YsfJsapiOrderController(IMQSender mqSender, Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            RequestKit requestKit,
            ILogger<AliJsapiOrderController> logger,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
            : base(mqSender, paymentServiceFactory, configContextQueryService, payOrderProcessService, requestKit, logger, mchPayPassageService, payOrderService, sysConfigService)
        {
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        [HttpPost, Route("api/pay/ysfJsapiOrder")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER)]
        public ActionResult<ApiRes> YsfJsapiOrder()
        {
            //获取参数 & 验证
            YsfJsapiOrderRQ bizRQ = GetRQByWithMchSign<YsfJsapiOrderRQ>();

            // 统一下单接口
            return UnifiedOrder(CS.PAY_WAY_CODE.YSF_JSAPI, bizRQ);
        }
    }
}
