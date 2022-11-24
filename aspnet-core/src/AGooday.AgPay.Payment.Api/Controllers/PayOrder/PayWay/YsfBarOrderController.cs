using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder.PayWay
{
    /// <summary>
    /// 云闪付 条码支付 controller
    /// </summary>
    public class YsfBarOrderController : AbstractPayOrderController
    {
        public YsfBarOrderController(IMQSender mqSender,
            Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            RequestIpUtil requestIpUtil,
            ILogger<AliBarOrderController> logger,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
            : base(mqSender, paymentServiceFactory, configContextQueryService, payOrderProcessService, requestIpUtil, logger, mchPayPassageService, payOrderService, sysConfigService)
        {
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/pay/ysfBarOrder")]
        public ActionResult<ApiRes> YsfBarOrder()
        {
            //获取参数 & 验证
            YsfBarOrderRQ bizRQ = GetRQByWithMchSign<YsfBarOrderRQ>();

            // 统一下单接口;
            return UnifiedOrder(CS.PAY_WAY_CODE.YSF_BAR, bizRQ);// "ALI_BAR";  //支付宝条码支付
        }
    }
}
