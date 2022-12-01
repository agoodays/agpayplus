using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    /// <summary>
    /// 微信支付 获取微信openID实现类
    /// </summary>
    public class WxPayChannelUserService : IChannelUserService
    {
        private readonly ILogger<WxPayChannelUserService> log;
        private readonly ConfigContextQueryService _configContextQueryService;

        /// <summary>
        /// 默认官方跳转地址
        /// </summary>
        private const string DEFAULT_OAUTH_URL = "https://open.weixin.qq.com/connect/oauth2/authorize";

        public WxPayChannelUserService(ILogger<WxPayChannelUserService> logger, ConfigContextQueryService configContextQueryService)
        {
            this.log = logger;
            _configContextQueryService = configContextQueryService;
        }

        public string BuildUserRedirectUrl(string callbackUrlEncode, MchAppConfigContext mchAppConfigContext)
        {
            string appId = null;
            string oauth2Url = "";
            if (mchAppConfigContext.IsIsvSubMch())
            {
                WxPayIsvParams wxpayIsvParams = (WxPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.WXPAY);
                if (wxpayIsvParams == null)
                {
                    throw new BizException("服务商微信支付接口没有配置！");
                }
                appId = wxpayIsvParams.AppId;
                oauth2Url = wxpayIsvParams.Oauth2Url;
            }
            else
            {
                //获取商户配置信息
                WxPayNormalMchParams normalMchParams = (WxPayNormalMchParams)_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);
                if (normalMchParams == null)
                {
                    throw new BizException("商户微信支付接口没有配置！");
                }
                appId = normalMchParams.AppId;
                oauth2Url = normalMchParams.Oauth2Url;
            }

            if (string.IsNullOrEmpty(oauth2Url))
            {
                oauth2Url = DEFAULT_OAUTH_URL;
            }
            string wxUserRedirectUrl = $"{oauth2Url}?appid={appId}&scope=snsapi_base&state=&redirect_uri={callbackUrlEncode}&response_type=code#wechat_redirect";
            log.LogInformation($"wxUserRedirectUrl={wxUserRedirectUrl}");
            return wxUserRedirectUrl;
        }

        public string GetChannelUserId(JObject reqParams, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                string code = reqParams.GetValue("code").ToString();
                WxServiceWrapper wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
                var options = new WechatApiClientOptions()
                {
                    AppId = wxServiceWrapper.Config.AppId,
                    AppSecret = wxServiceWrapper.Config.AppSecret,
                };
                var client = new WechatApiClient(options);
                var request = new SnsOAuth2AccessTokenRequest();
                request.Code = code;
                return client.ExecuteSnsOAuth2AccessTokenAsync(request).Result.OpenId;
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message);
                return null;
            }
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }
    }
}
