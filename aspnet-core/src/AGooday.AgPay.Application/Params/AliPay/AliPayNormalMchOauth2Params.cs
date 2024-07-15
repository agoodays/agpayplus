using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝 isv oauth2参数定义
    /// </summary>
    public class AliPayNormalMchOauth2Params : NormalMchOauth2Params
    {
        /// <summary>
        /// 环境配置
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// 合作伙伴身份（PID）
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 应用AppID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用私钥
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public string AlipayPublicKey { get; set; }

        /// <summary>
        /// 接口签名方式(推荐使用RSA2)
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// 是否使用证书方式
        /// </summary>
        public byte? UseCert { get; set; }

        /// <summary>
        /// 应用公钥证书（.crt格式）
        /// </summary>
        public string AppPublicCert { get; set; }

        /// <summary>
        /// 支付宝公钥证书（.crt格式）
        /// </summary>
        public string AlipayPublicCert { get; set; }

        /// <summary>
        /// 支付宝根证书（.crt格式）
        /// </summary>
        public string AlipayRootCert { get; set; }

        /// <summary>
        /// 小程序参数配置
        /// </summary>
        public AliLiteParams LiteParams { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(PrivateKey))
            {
                PrivateKey = PrivateKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(AlipayPublicKey))
            {
                AlipayPublicKey = AlipayPublicKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(LiteParams?.PrivateKey))
            {
                LiteParams.PrivateKey = LiteParams.PrivateKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(LiteParams?.AlipayPublicKey))
            {
                LiteParams.AlipayPublicKey = LiteParams.AlipayPublicKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
