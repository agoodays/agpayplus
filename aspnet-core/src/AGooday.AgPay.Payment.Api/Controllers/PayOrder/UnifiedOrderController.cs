using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 统一下单
    /// </summary>
    [ApiController]
    public class UnifiedOrderController : AbstractPayOrderController
    {
        public UnifiedOrderController(Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
            : base(paymentServiceFactory, configContextQueryService, payOrderProcessService, mchPayPassageService, payOrderService, sysConfigService)
        {
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/unifiedOrder")]
        public ActionResult<ApiRes> UnifiedOrder()
        {
            return Ok();
        }
    }
}
