using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.WxPay
{
    /// <summary>
    /// 微信官方支付 配置参数
    /// </summary>
    public class WxPayNormalMchParams : NormalMchParams
    {
        /// <summary>
        /// 应用App ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 微信支付商户号
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// oauth2地址
        /// </summary>
        public string Oauth2Url { get; set; }

        /// <summary>
        /// API密钥
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 微信支付API版本
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// API V3秘钥
        /// </summary>
        public string ApiV3Key { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// API证书(.p12格式)
        /// </summary>
        public string Cert { get; set; }

        /// <summary>
        /// 私钥文件(.pem格式)
        /// </summary>
        public string ApiClientKey { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(AppSecret))
            {
                AppSecret = AppSecret.Mask();
            }
            if (!string.IsNullOrWhiteSpace(Key))
            {
                Key = Key.Mask();
            }
            if (!string.IsNullOrWhiteSpace(ApiV3Key))
            {
                ApiV3Key = ApiV3Key.Mask();
            }
            if (!string.IsNullOrWhiteSpace(SerialNo))
            {
                SerialNo = SerialNo.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
