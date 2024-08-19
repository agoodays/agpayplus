using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using Aop.Api.Request;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付宝 获取用户ID实现类
    /// </summary>
    public class AliPayChannelUserService : IChannelUserService
    {
        private readonly ILogger<AliPayChannelUserService> _logger;
        private readonly ConfigContextQueryService _configContextQueryService;

        public AliPayChannelUserService(ILogger<AliPayChannelUserService> logger,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _configContextQueryService = configContextQueryService;
        }

        public AliPayChannelUserService()
        {
        }

        public string BuildUserRedirectUrl(string callbackUrlEncode, string oauth2InfoId, string wayCode, MchAppConfigContext mchAppConfigContext)
        {
            string appId;
            byte? sandbox;

            if (mchAppConfigContext.IsIsvSubMch())
            {
                var isvOauth2Params = (AliPayIsvOauth2Params)_configContextQueryService.QueryIsvOauth2Params(mchAppConfigContext.MchInfo.IsvNo, oauth2InfoId, GetIfCode());
                if (isvOauth2Params == null)
                {
                    throw new BizException("服务商支付宝Oauth2配置没有配置！");
                }
                appId = isvOauth2Params.AppId;
                sandbox = isvOauth2Params.Sandbox;
                if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_LITE))
                {
                    appId = isvOauth2Params.LiteParams.AppId;
                    sandbox = isvOauth2Params.LiteParams.Sandbox;
                    var isvSubMchOauth2Params = (AliPayIsvSubMchOauth2Params)_configContextQueryService.QueryIsvSubMchOauth2Params(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                    if (isvSubMchOauth2Params.IsUseSubmchAccount.Equals(CS.YES))
                    {
                        appId = isvSubMchOauth2Params.LiteParams.AppId;
                        sandbox = isvSubMchOauth2Params.LiteParams.Sandbox;
                    }
                }
            }
            else
            {
                //获取商户配置信息
                var normalMchOauth2Params = (AliPayNormalMchOauth2Params)_configContextQueryService.QueryNormalMchOauth2Params(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                if (normalMchOauth2Params == null)
                {
                    throw new BizException("商户支付宝Oauth2配置没有配置！");
                }
                appId = normalMchOauth2Params.AppId;
                sandbox = normalMchOauth2Params.Sandbox;
                if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_LITE))
                {
                    appId = normalMchOauth2Params.LiteParams.AppId;
                    sandbox = normalMchOauth2Params.LiteParams.Sandbox;
                }
            }
            string oauthUrl = AliPayConfig.PROD_OAUTH_URL;
            if (sandbox == CS.YES)
            {
                oauthUrl = AliPayConfig.SANDBOX_OAUTH_URL;
            }
            string alipayUserRedirectUrl = string.Format(oauthUrl, appId, callbackUrlEncode);
            _logger.LogInformation($"alipayUserRedirectUrl={alipayUserRedirectUrl}");
            return alipayUserRedirectUrl;
        }

        public string GetChannelUserId(JObject reqParams, string oauth2InfoId, string wayCode, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                AliPayClientWrapper alipayClientWrapper;

                if (mchAppConfigContext.IsIsvSubMch())
                {
                    var isvOauth2Params = (AliPayIsvOauth2Params)_configContextQueryService.QueryIsvOauth2Params(mchAppConfigContext.MchInfo.IsvNo, oauth2InfoId, GetIfCode());

                    if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_LITE))
                    {
                        var isvSubMchOauth2Params = (AliPayIsvSubMchOauth2Params)_configContextQueryService.QueryIsvSubMchOauth2Params(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                        if (isvSubMchOauth2Params.IsUseSubmchAccount.Equals(CS.YES))
                        {
                            alipayClientWrapper = AliPayClientWrapper.BuildAlipayClientWrapper(isvSubMchOauth2Params.LiteParams);
                        }
                        else
                        {
                            alipayClientWrapper = AliPayClientWrapper.BuildAlipayClientWrapper(isvOauth2Params.LiteParams);
                        }
                    }
                    else
                    {
                        alipayClientWrapper = AliPayClientWrapper.BuildAlipayClientWrapper(isvOauth2Params);
                    }

                }
                else
                {
                    var normalMchOauth2Params = (AliPayNormalMchOauth2Params)_configContextQueryService.QueryNormalMchOauth2Params(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                    if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_LITE))
                    {
                        alipayClientWrapper = AliPayClientWrapper.BuildAlipayClientWrapper(normalMchOauth2Params.LiteParams);
                    }
                    else
                    {
                        alipayClientWrapper = AliPayClientWrapper.BuildAlipayClientWrapper(normalMchOauth2Params);
                    }
                }
                string authCode = reqParams.GetValue("auth_code").ToString();
                //通过code 换取openId
                AlipaySystemOauthTokenRequest request = new AlipaySystemOauthTokenRequest();
                request.Code = authCode;
                request.GrantType = "authorization_code";
                return alipayClientWrapper.Execute(request).UserId;
            }
            catch (ChannelException e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }
    }
}
