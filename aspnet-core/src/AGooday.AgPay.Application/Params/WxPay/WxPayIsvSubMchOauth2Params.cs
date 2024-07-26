using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.WxPay
{
    /// <summary>
    /// 微信 oauth2 配置参数
    /// </summary>
    public class WxPayIsvSubMchOauth2Params : IsvSubMchOauth2Params
    {
        /// <summary>
        /// 特约商户小程序支付跳转的选择
        /// </summary>
        public byte? IsUseSubmchAccount { get; set; }
        /// <summary>
        /// 特约商户的小程序AppID
        /// </summary>
        public string LiteAppId { get; set; }
        /// <summary>
        /// 特约商户的小程序appSecret
        /// </summary>
        public string LiteAppSecret { get; set; }
        /// <summary>
        /// 特约商户的小程序原始ID
        /// </summary>
        public string LiteGhid { get; set; }
        /// <summary>
        /// 特约商户的小程序版本
        /// </summary>
        public string LiteEnv { get; set; }
        /// <summary>
        /// 特约商户的小程序路径
        /// </summary>
        public string LitePagePath { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(LiteAppSecret))
            {
                LiteAppSecret = LiteAppSecret.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
