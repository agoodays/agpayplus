using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace AGooday.AgPay.Payment.Api.Controllers.Qr
{
    [Route("api/channelUserId")]
    [ApiController]
    public class ChannelUserIdController : AbstractPayOrderController
    {
        public ChannelUserIdController(IMQSender mqSender, 
            Func<string, IPaymentService> paymentServiceFactory, 
            ConfigContextQueryService configContextQueryService, 
            PayOrderProcessService payOrderProcessService, 
            RequestIpUtil requestIpUtil, 
            ILogger<AbstractPayOrderController> logger, 
            IMchPayPassageService mchPayPassageService, 
            IPayOrderService payOrderService, 
            ISysConfigService sysConfigService) 
            : base(mqSender, paymentServiceFactory, configContextQueryService, payOrderProcessService, requestIpUtil, logger, mchPayPassageService, payOrderService, sysConfigService)
        {
        }

        [Route("jump")]
        public ActionResult Jump()
        {

            return Redirect("");
        }

        /// <summary>
        /// 根据UA获取支付接口
        /// </summary>
        /// <returns></returns>
        private string GetIfCodeByUA()
        {
            string ua = Request.Headers["User-Agent"].FirstOrDefault();

            // 无法识别扫码客户端
            if (string.IsNullOrEmpty(ua))
            {
                return null;
            }

            if (ua.Contains("Alipay"))
            {
                return CS.IF_CODE.ALIPAY;  //支付宝服务窗支付
            }
            else if (ua.Contains("MicroMessenger"))
            {
                return CS.IF_CODE.WXPAY;
            }
            return null;
        }
    }
}
