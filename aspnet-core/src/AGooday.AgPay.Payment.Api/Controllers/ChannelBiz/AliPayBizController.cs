﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Controllers.ChannelBiz
{
    /// <summary>
    /// 渠道侧自定义业务
    /// </summary>
    [Route("api/channelbiz/alipay")]
    [ApiController]
    public class AliPayBizController : Controller
    {
        private readonly ILogger<AliPayBizController> _logger;
        private readonly ConfigContextQueryService configContextQueryService;
        private readonly IPayInterfaceConfigService payInterfaceConfigService;
        private readonly ISysConfigService sysConfigService;
        private readonly IMchAppService mchAppService;
        private readonly IMQSender mqSender;

        public AliPayBizController(ILogger<AliPayBizController> logger,
            ConfigContextQueryService configContextQueryService,
            IPayInterfaceConfigService payInterfaceConfigService,
            ISysConfigService sysConfigService,
            IMchAppService mchAppService,
            IMQSender mqSender)
        {
            _logger = logger;
            this.configContextQueryService = configContextQueryService;
            this.payInterfaceConfigService = payInterfaceConfigService;
            this.sysConfigService = sysConfigService;
            this.mchAppService = mchAppService;
            this.mqSender = mqSender;
        }

        /// <summary>
        /// 跳转到支付宝的授权页面 （统一从pay项目获取到isv配置信息）
        /// isvAndMchNo 格式:  ISVNO_MCHAPPID
        /// example: https://pay.agpay.cn/api/channelbiz/alipay/redirectAppToAppAuth/V1623998765_60cc41694ee0e6685f57eb1f
        /// </summary>
        /// <param name="isvAndMchAppId"></param>
        [HttpGet, Route("redirectAppToAppAuth/{isvAndMchAppId}")]
        public IActionResult RedirectAppToAppAuth(string isvAndMchAppId)
        {
            string isvNo = isvAndMchAppId.Split("_")[0];

            AliPayIsvParams alipayIsvParams = (AliPayIsvParams)configContextQueryService.QueryIsvParams(isvNo, CS.IF_CODE.ALIPAY);
            var isSandbox = alipayIsvParams.Sandbox != null && alipayIsvParams.Sandbox == CS.YES;

            string oauthUrl = isSandbox ? AliPayConfig.SANDBOX_APP_TO_APP_AUTH_URL : AliPayConfig.PROD_APP_TO_APP_AUTH_URL;

            string redirectUrl = sysConfigService.GetDBApplicationConfig().PaySiteUrl + "/api/channelbiz/alipay/appToAppAuthCallback";

            return Redirect(string.Format(oauthUrl, alipayIsvParams.AppId, URLUtil.EncodeAll(redirectUrl), isvAndMchAppId));
        }

        [HttpGet, Route("appToAppAuthCallback")]
        public async Task<IActionResult> AppToAppAuthCallbackAsync(string state, string app_auth_code)
        {
            string errMsg = null;
            bool isAlipaySysAuth = true; //是否 服务商登录支付宝后台系统发起的商户授权， 此时无法获取authCode和商户的信息。

            try
            {
                // isvAndMchAppId 格式:  ISVNO_MCHAPPID,  如果isvAndMchNo为空说明是： 支付宝后台的二维码授权之后的跳转链接。
                string isvAndMchAppId = state;
                string appAuthCode = app_auth_code; // 支付宝授权code

                if (StringUtil.IsAllNotNullOrWhiteSpace(isvAndMchAppId, appAuthCode))
                {
                    isAlipaySysAuth = false;
                    string isvNo = isvAndMchAppId.Split("_")[0];
                    string mchAppId = isvAndMchAppId.Split("_")[1];

                    MchAppDto mchApp = await mchAppService.GetByIdAsync(mchAppId);

                    MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(mchApp.MchNo, mchAppId);
                    AliPayClientWrapper alipayClientWrapper = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext);

                    AlipayOpenAuthTokenAppRequest request = new AlipayOpenAuthTokenAppRequest();
                    AlipayOpenAuthTokenAppModel model = new AlipayOpenAuthTokenAppModel();
                    model.GrantType = "authorization_code";
                    model.Code = appAuthCode;
                    request.SetBizModel(model);

                    // expiresIn: 该字段已作废，应用令牌长期有效，接入方不需要消费该字段
                    // reExpiresIn: 刷新令牌的有效时间（从接口调用时间作为起始时间），单位到秒
                    // DateUtil.offsetSecond(new Date(), Integer.parseInt(resp.getExpiresIn()));
                    AlipayOpenAuthTokenAppResponse resp = alipayClientWrapper.Execute(request);
                    if (resp.IsError)
                    {
                        throw new BizException(AliPayKit.AppendErrMsg(resp.Msg, resp.SubMsg));
                    }
                    JObject ifParams = new JObject();
                    ifParams.Add("appAuthToken", resp.AppAuthToken);
                    ifParams.Add("refreshToken", resp.AppRefreshToken);
                    ifParams.Add("expireTimestamp", resp.ExpiresIn);

                    PayInterfaceConfigDto dbRecord = payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.MCH_APP, mchAppId, CS.IF_CODE.ALIPAY);

                    if (dbRecord != null)
                    {
                        dbRecord.Id = dbRecord.Id;
                        dbRecord.IfParams = ifParams.ToString();
                        payInterfaceConfigService.Update(dbRecord);
                    }
                    else
                    {
                        dbRecord = new PayInterfaceConfigDto();
                        dbRecord.InfoType = CS.INFO_TYPE.MCH_APP;
                        dbRecord.InfoId = mchAppId;
                        dbRecord.IfCode = CS.IF_CODE.ALIPAY;
                        dbRecord.IfParams = ifParams.ToString();
                        dbRecord.IfRate = 0.006M; //默认费率
                        dbRecord.State = CS.YES;
                        dbRecord.CreatedBy = "SYS";
                        dbRecord.CreatedUid = 0L;
                        payInterfaceConfigService.Add(dbRecord);
                    }

                    // 更新应用配置信息
                    mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, null, mchApp.MchNo, mchApp.AppId));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"error");
                errMsg = !string.IsNullOrWhiteSpace(e.Message) ? e.Message : "系统异常！";
            }
            ViewBag.ErrMsg = errMsg;
            ViewBag.IsAlipaySysAuth = isAlipaySysAuth;
            return View("~/Views/Channel/Alipay/IsvSubMchAuth.cshtml");
        }

        [HttpGet, Route("appGatewayMsgReceive")]
        public ActionResult AlipayAppGatewayMsgReceive()
        {
            JObject reqJSON = GetReqParamJson();

            // 获取到报文信息，然后转发到对应的ctrl
            _logger.LogInformation($"支付宝应用网关接收消息参数：{reqJSON}");

            // 分账交易通知
            if ("alipay.trade.order.settle.notify".Equals(reqJSON.GetValue("msg_method").ToString()))
            {
                // 直接转发到分账通知的URL去。
                return Redirect($"/api/divisionRecordChannelNotify/{CS.IF_CODE.ALIPAY}");
            }

            throw new BizException($"无此事件[{reqJSON.GetValue("msg_method")}]处理器");
        }

        /// <summary>
        /// 获取json格式的请求参数
        /// </summary>
        /// <returns></returns>
        protected JObject GetReqParamJson()
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
