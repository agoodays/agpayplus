using System.Text;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Models;
using AGooday.AgPay.Merchant.Api.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayTest
{
    [Route("api/anon/paytestNotify")]
    [ApiController, AllowAnonymous, NoLog]
    public class PayTestNotifyController : CommonController
    {
        private readonly IMchAppService _mchAppService;
        private readonly WsPayOrderServer _wsPayOrderServer;

        public PayTestNotifyController(ILogger<PayTestNotifyController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchAppService mchAppService,
            WsPayOrderServer wsPayOrderServer)
            : base(logger, cacheService, authService)
        {
            _mchAppService = mchAppService;
            _wsPayOrderServer = wsPayOrderServer;
        }

        [HttpPost, Route("payOrder")]
        public async Task<ActionResult> PayOrderNotifyAsync(PayOrderNotifyModel payOrderNotify)
        {
            var mchApp = await _mchAppService.GetByIdAsync(payOrderNotify.AppId);
            if (mchApp == null || !mchApp.MchNo.Equals(payOrderNotify.MchNo))
            {
                return Content("app is not exists");
            }
            var jsonparams = GetReqParamJson();
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
            await WsPayOrderServer.BroadcastToOrderAsync(payOrderNotify.PayOrderId, msg.ToString(Formatting.None));

            return Content("SUCCESS");
        }

        /// <summary>
        /// 获取json格式的请求参数
        /// </summary>
        /// <returns></returns>
        private JObject GetReqParamJson()
        {
            Request.EnableBuffering();

            string body = "";
            var stream = Request.Body;
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
                {
                    body = reader.ReadToEnd();
                }
                stream.Seek(0, SeekOrigin.Begin);
            }

            return JObject.Parse(body);
        }
    }
}
