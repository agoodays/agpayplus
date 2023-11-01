using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.WxPay
{
    /// <summary>
    /// 微信 oauth2 配置参数
    /// </summary>
    public class WxPayNormalMchOauth2Params : NormalMchOauth2Params
    {
        /// <summary>
        /// 商户的公众号AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 商户的公众号AppSecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// oauth2地址（置空将使用官方）
        /// </summary>
        public string Oauth2Url { get; set; }
        /// <summary>
        /// 商户的小程序AppID
        /// </summary>
        public string LiteAppId { get; set; }
        /// <summary>
        /// 商户的小程序appSecret
        /// </summary>
        public string LiteAppSecret { get; set; }
        /// <summary>
        /// 商户的小程序原始ID
        /// </summary>
        public string LiteGhid { get; set; }
        /// <summary>
        /// 商户的小程序版本
        /// </summary>
        public string LiteEnv { get; set; }
        /// <summary>
        /// 商户的小程序路径
        /// </summary>
        public string LitePagePath { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(AppSecret))
            {
                AppSecret = AppSecret.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
