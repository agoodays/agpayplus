using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Models;
using AGooday.AgPay.Merchant.Api.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayTest
{
    [Route("api/anon/paytestNotify")]
    [ApiController, AllowAnonymous, NoLog]
    public class PayTestNotifyController : ControllerBase
    {
        private readonly ILogger<PayTestNotifyController> _logger;
        private readonly IMchAppService _mchAppService;
        private readonly WsPayOrderServer _wsPayOrderServer;

        public PayTestNotifyController(ILogger<PayTestNotifyController> logger, IMchAppService mchAppService, WsPayOrderServer wsPayOrderServer)
        {
            _logger = logger;
            _mchAppService = mchAppService;
            _wsPayOrderServer = wsPayOrderServer;
        }

        [HttpPost, Route("payOrder")]
        public async Task<ActionResult> PayOrderNotify(PayOrderNotifyModel payOrderNotify)
        {
            var mchApp = _mchAppService.GetById(payOrderNotify.AppId);
            if (mchApp == null || !mchApp.MchNo.Equals(payOrderNotify.MchNo))
            {
                return Content("app is not exists");
            }
            var jsonparams = JObject.FromObject(payOrderNotify);
            jsonparams.Remove("sign");

            if (!AgPayUtil.GetSign(jsonparams, mchApp.AppSecret).Equals(payOrderNotify.Sign, StringComparison.OrdinalIgnoreCase))
            {
                return Content("sign fail");
            }

            JObject msg = new JObject();
            msg.Add("state", payOrderNotify.State);
            msg.Add("errCode", payOrderNotify.ErrCode);
            msg.Add("errMsg", payOrderNotify.ErrMsg);

            //推送到前端
            await _wsPayOrderServer.SendMsgByOrderId(payOrderNotify.PayOrderId, msg.ToString());

            return Content("SUCCESS");
        }
    }
}
