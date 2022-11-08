using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using log4net;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using Newtonsoft.Json.Linq;
using AGooday.AgPay.Components.MQ.Vender;

namespace AGooday.AgPay.Payment.Api.Controllers.ChannelBiz
{
    [Route("api/channelbiz/alipay")]
    [ApiController]
    public class AliPayBizController : Controller
    {
        private readonly ILogger<AliPayBizController> log;
        private readonly ConfigContextQueryService configContextQueryService;
        private readonly IPayInterfaceConfigService payInterfaceConfigService;
        private readonly ISysConfigService sysConfigService;
        private readonly IMchAppService mchAppService;
        private readonly IMQSender mqSender;

        public AliPayBizController(ILogger<AliPayBizController> log, 
            ConfigContextQueryService configContextQueryService, 
            IPayInterfaceConfigService payInterfaceConfigService, 
            ISysConfigService sysConfigService, 
            IMchAppService mchAppService, 
            IMQSender mqSender)
        {
            this.log = log;
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
        public IActionResult AppToAppAuthCallback(string state, string app_auth_code)
        {
            string errMsg = null;
            bool isAlipaySysAuth = true; //是否 服务商登录支付宝后台系统发起的商户授权， 此时无法获取authCode和商户的信息。

            try
            {
                // isvAndMchAppId 格式:  ISVNO_MCHAPPID,  如果isvAndMchNo为空说明是： 支付宝后台的二维码授权之后的跳转链接。
                string isvAndMchAppId = state;
                string appAuthCode = app_auth_code; // 支付宝授权code

                if (!string.IsNullOrWhiteSpace(isvAndMchAppId) && !string.IsNullOrWhiteSpace(appAuthCode))
                {
                    isAlipaySysAuth = false;
                    string isvNo = isvAndMchAppId.Split("_")[0];
                    string mchAppId = isvAndMchAppId.Split("_")[1];

                    MchAppDto mchApp = mchAppService.GetById(mchAppId);

                    MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(mchApp.MchNo, mchAppId);
                    AlipayClientWrapper alipayClientWrapper = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext);

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

                    PayInterfaceConfigDto dbRecord = payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_MCH_APP, mchAppId, CS.IF_CODE.ALIPAY);

                    if (dbRecord != null)
                    {
                        dbRecord.Id = dbRecord.Id;
                        dbRecord.IfParams = ifParams.ToString();
                        payInterfaceConfigService.Update(dbRecord);
                    }
                    else
                    {
                        dbRecord = new PayInterfaceConfigDto();
                        dbRecord.InfoType = CS.INFO_TYPE_MCH_APP;
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
                    mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, mchApp.MchNo, mchApp.AppId));
                }
            }
            catch (Exception e)
            {
                log.LogError("error", e);
                errMsg = !string.IsNullOrWhiteSpace(e.Message) ? e.Message : "系统异常！";
            }
            ViewBag.ErrMsg = errMsg;
            ViewBag.IsAlipaySysAuth = isAlipaySysAuth;
            return View("~/Views/Channel/Alipay/IsvSubMchAuth.cshtml");
        }
    }
}
