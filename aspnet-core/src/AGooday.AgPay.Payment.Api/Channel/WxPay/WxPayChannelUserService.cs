using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    public class WxPayChannelUserService : IChannelUserService
    {
        private readonly ConfigContextQueryService _configContextQueryService;

        /// <summary>
        /// 默认官方跳转地址
        /// </summary>
        private const string DEFAULT_OAUTH_URL = "https://open.weixin.qq.com/connect/oauth2/authorize";

        public WxPayChannelUserService(ConfigContextQueryService configContextQueryService)
        {
            _configContextQueryService = configContextQueryService;
        }

        public string BuildUserRedirectUrl(string callbackUrlEncode, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }

        public string GetChannelUserId(JObject reqParams, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                string code = reqParams.GetValue("code").ToString();
                WxServiceWrapper wxServiceWrapper = _configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
                return string.Empty;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }
    }
}
