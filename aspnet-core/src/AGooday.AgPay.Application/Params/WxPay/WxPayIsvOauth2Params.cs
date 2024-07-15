using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.WxPay
{
    /// <summary>
    /// 微信 isv oauth2参数定义
    /// </summary>
    public class WxPayIsvOauth2Params : IsvOauth2Params
    {
        /// <summary>
        /// 服务商的公众号AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 服务商的公众号AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Oauth2地址（置空将使用官方）
        /// </summary>
        public string Oauth2Url { get; set; }

        /// <summary>
        /// 服务商的小程序AppID
        /// </summary>
        public string LiteAppId { get; set; }

        /// <summary>
        /// 服务商的小程序AppSecret
        /// </summary>
        public string LiteAppSecret { get; set; }

        /// <summary>
        /// 服务商的小程序原始ID
        /// </summary>
        public string LiteGhid { get; set; }

        /// <summary>
        /// 服务商的小程序版本
        /// </summary>
        public string LiteEnv { get; set; }

        /// <summary>
        /// 服务商的小程序路径
        /// </summary>
        public string LitePagePath { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(AppSecret))
            {
                AppSecret = AppSecret.Mask();
            }
            if (!string.IsNullOrWhiteSpace(LiteAppSecret))
            {
                LiteAppSecret = LiteAppSecret.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
