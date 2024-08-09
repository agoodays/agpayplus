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
    /// 云闪付 条码支付 controller
    /// </summary>
    public class YsfBarOrderController : AbstractPayOrderController
    {
        public YsfBarOrderController(ILogger<YsfBarOrderController> logger,
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
        [HttpPost, Route("api/pay/ysfBarOrder")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER)]
        public ActionResult<ApiRes> YsfBarOrder()
        {
            //获取参数 & 验证
            YsfBarOrderRQ bizRQ = GetRQByWithMchSign<YsfBarOrderRQ>();

            // 统一下单接口;
            return UnifiedOrder(CS.PAY_WAY_CODE.YSF_BAR, bizRQ);// "ALI_BAR";  //支付宝条码支付
        }
    }
}
