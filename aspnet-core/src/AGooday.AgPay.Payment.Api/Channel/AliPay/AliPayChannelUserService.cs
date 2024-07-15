﻿using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api.Request;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    /// <summary>
    /// 支付宝 获取用户ID实现类
    /// </summary>
    public class AliPayChannelUserService : IChannelUserService
    {
        private readonly ILogger<AliPayChannelUserService> _logger;
        private readonly ConfigContextQueryService configContextQueryService;

        public AliPayChannelUserService(ILogger<AliPayChannelUserService> logger, 
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            this.configContextQueryService = configContextQueryService;
        }

        public string BuildUserRedirectUrl(string callbackUrlEncode, MchAppConfigContext mchAppConfigContext)
        {
            string oauthUrl = AliPayConfig.PROD_OAUTH_URL;
            string appId = null;

            if (mchAppConfigContext.IsIsvSubMch())
            {
                AliPayIsvParams isvParams = (AliPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());
                if (isvParams == null)
                {
                    throw new BizException("服务商支付宝接口没有配置！");
                }
                appId = isvParams.AppId;
            }
            else
            {
                //获取商户配置信息
                AliPayNormalMchParams normalMchParams = (AliPayNormalMchParams)configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
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
            _logger.LogInformation($"alipayUserRedirectUrl={alipayUserRedirectUrl}");
            return alipayUserRedirectUrl;
        }

        public string GetChannelUserId(JObject reqParams, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                string authCode = reqParams.GetValue("auth_code").ToString();
                //通过code 换取openId
                AlipaySystemOauthTokenRequest request = new AlipaySystemOauthTokenRequest();
                request.Code = authCode;
                request.GrantType = "authorization_code";
                return configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request).UserId;
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
