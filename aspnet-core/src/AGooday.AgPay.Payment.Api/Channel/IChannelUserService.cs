using AGooday.AgPay.Payment.Api.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 301方式获取渠道侧用户ID， 如微信openId 支付宝的userId等
    /// </summary>
    public interface IChannelUserService
    {
        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 获取重定向地址
        /// </summary>
        /// <param name="callbackUrlEncode"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        string BuildUserRedirectUrl(string callbackUrlEncode, MchAppConfigContext mchAppConfigContext);

        /// <summary>
        /// 获取渠道用户ID
        /// </summary>
        /// <param name="reqParams"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        string GetChannelUserId(JObject reqParams, MchAppConfigContext mchAppConfigContext);
    }
}
