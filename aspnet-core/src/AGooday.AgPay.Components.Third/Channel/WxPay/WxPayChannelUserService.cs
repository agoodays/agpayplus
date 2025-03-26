using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using Newtonsoft.Json.Linq;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 微信支付 获取微信openID实现类
    /// </summary>
    public class WxPayChannelUserService : IChannelUserService
    {
        private readonly ILogger<WxPayChannelUserService> _logger;
        private readonly ConfigContextQueryService _configContextQueryService;

        /// <summary>
        /// 默认官方跳转地址
        /// </summary>
        private const string DEFAULT_OAUTH_URL = "https://open.weixin.qq.com/connect/oauth2/authorize";

        public WxPayChannelUserService(ILogger<WxPayChannelUserService> logger, ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _configContextQueryService = configContextQueryService;
        }

        public WxPayChannelUserService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public async Task<string> BuildUserRedirectUrlAsync(string callbackUrlEncode, string oauth2InfoId, string wayCode, MchAppConfigContext mchAppConfigContext)
        {
            var (appId, _, oauth2Url) = await GetOauth2ParamsAsync(oauth2InfoId, wayCode, mchAppConfigContext);

            if (string.IsNullOrEmpty(oauth2Url))
            {
                oauth2Url = DEFAULT_OAUTH_URL;
            }
            string wxUserRedirectUrl = $"{oauth2Url}?appid={appId}&scope=snsapi_base&state=&redirect_uri={callbackUrlEncode}&response_type=code#wechat_redirect";
            _logger.LogInformation("wxUserRedirectUrl={wxUserRedirectUrl}", wxUserRedirectUrl);
            //_logger.LogInformation($"wxUserRedirectUrl={wxUserRedirectUrl}");
            return wxUserRedirectUrl;
        }

        public async Task<string> GetChannelUserIdAsync(JObject reqParams, string oauth2InfoId, string wayCode, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                string code = reqParams.GetValue("code").ToString();
                var (appId, appSecret, _) = await GetOauth2ParamsAsync(oauth2InfoId, wayCode, mchAppConfigContext);
                var options = new WechatApiClientOptions()
                {
                    AppId = appId,
                    AppSecret = appSecret,
                };
                var client = new WechatApiClient(options);
                var request = new SnsOAuth2AccessTokenRequest();
                request.Code = code;
                return client.ExecuteSnsOAuth2AccessTokenAsync(request).Result.OpenId;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<(string appId, string appSecret, string oauth2Url)> GetOauth2ParamsAsync(string oauth2InfoId, string wayCode, MchAppConfigContext mchAppConfigContext)
        {
            string appId, appSecret, oauth2Url;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvOauth2Params = (WxPayIsvOauth2Params)await _configContextQueryService.QueryIsvOauth2ParamsAsync(mchAppConfigContext.MchInfo.IsvNo, oauth2InfoId, GetIfCode());
                if (isvOauth2Params == null)
                {
                    throw new BizException("服务商微信Oauth2配置没有配置！");
                }
                appId = isvOauth2Params.AppId;
                appSecret = isvOauth2Params.AppSecret;
                oauth2Url = isvOauth2Params.Oauth2Url;
                if (wayCode.Equals(CS.PAY_WAY_CODE.WX_LITE))
                {
                    appId = isvOauth2Params.LiteAppId;
                    appSecret = isvOauth2Params.LiteAppSecret;
                    var isvSubMchOauth2Params = (WxPayIsvSubMchOauth2Params)await _configContextQueryService.QueryIsvSubMchOauth2ParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                    if (isvSubMchOauth2Params.IsUseSubmchAccount.Equals(CS.YES))
                    {
                        appId = isvSubMchOauth2Params.LiteAppId;
                        appSecret = isvSubMchOauth2Params.LiteAppSecret;
                    }
                }
            }
            else
            {
                //获取商户配置信息
                var normalMchOauth2Params = (WxPayNormalMchOauth2Params)await _configContextQueryService.QueryNormalMchOauth2ParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                if (normalMchOauth2Params == null)
                {
                    throw new BizException("商户微信Oauth2配置没有配置！");
                }
                appId = normalMchOauth2Params.AppId;
                appSecret = normalMchOauth2Params.AppSecret;
                oauth2Url = normalMchOauth2Params.Oauth2Url;
                if (wayCode.Equals(CS.PAY_WAY_CODE.WX_LITE))
                {
                    appId = normalMchOauth2Params.LiteAppId;
                    appSecret = normalMchOauth2Params.LiteAppSecret;
                }
            }
            return (appId, appSecret, oauth2Url);
        }
    }
}
