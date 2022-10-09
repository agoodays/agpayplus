using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using log4net;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    public class AliPayChannelUserService : IChannelUserService
    {
        private readonly ConfigContextQueryService _configContextQueryService;

        public AliPayChannelUserService(ConfigContextQueryService configContextQueryService)
        {
            _configContextQueryService = configContextQueryService;
        }

        public string BuildUserRedirectUrl(string callbackUrlEncode, MchAppConfigContext mchAppConfigContext)
        {
            string oauthUrl = AliPayConfig.PROD_OAUTH_URL;
            string appId = null;

            if (mchAppConfigContext.IsIsvsubMch())
            {
                AliPayIsvParams isvParams = (AliPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());
                if (isvParams == null)
                {
                    throw new BizException("服务商支付宝接口没有配置！");
                }
                appId = isvParams.AppId;
            }
            else
            {
                //获取商户配置信息
                AliPayNormalMchParams normalMchParams = (AliPayNormalMchParams)_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                if (normalMchParams == null)
                {
                    throw new BizException("商户支付宝接口没有配置！");
                }
                appId = normalMchParams.AppId;
                if (normalMchParams.Sandbox != null && normalMchParams.Sandbox == CS.YES)
                {
                    oauthUrl = AliPayConfig.SANDBOX_OAUTH_URL;
                }
            }
            string alipayUserRedirectUrl = string.Format(oauthUrl, appId, callbackUrlEncode);
            //log.info("alipayUserRedirectUrl={}", alipayUserRedirectUrl);
            return alipayUserRedirectUrl;
        }

        public string GetChannelUserId(JObject reqParams, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                //通过code 换取openId
                string authCode = reqParams.GetValue("auth_code").ToString();
                return string.Empty;
            }
            catch (ChannelException e)
            {
                return null;
            }
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }
    }
}
