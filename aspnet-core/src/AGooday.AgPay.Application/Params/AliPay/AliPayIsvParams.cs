using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝 isv参数定义
    /// </summary>
    public class AliPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// pid
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// privateKey
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// alipayPublicKey
        /// </summary>
        public string AlipayPublicKey { get; set; }

        /// <summary>
        /// 签名方式
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// 是否使用证书方式
        /// </summary>
        public byte UseCert { get; set; }

        /// <summary>
        /// app 证书
        /// </summary>
        public string AppPublicCert { get; set; }

        /// <summary>
        /// 支付宝公钥证书（.crt格式）
        /// </summary>
        public string AlipayPublicCert { get; set; }

        /// <summary>
        /// 支付宝根证书
        /// </summary>
        public string AlipayRootCert { get; set; }

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
            return JsonConvert.SerializeObject(this);
        }
    }
}
