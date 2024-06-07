using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.YsePay
{
    /// <summary>
    /// 银盛 配置信息
    /// </summary>
    public class YsePayIsvParams : IsvParams
    {
        /// <summary>
        /// 发起方商户号（服务商商户号）
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// 业务代码
        /// </summary>
        public string BusinessCode { get; set; }

        /// <summary>
        /// 私钥密码
        /// </summary>
        public string PrivateKeyPassword { get; set; }

        /// <summary>
        /// 私钥证书（.pfx/.sm2）
        /// </summary>
        public string PrivateKeyFile { get; set; }

        /// <summary>
        /// 银盛公钥证书（.cer格式）
        /// </summary>
        public string PublicKeyFile { get; set; }

        /// <summary>
        /// 微信渠道号[服务商通过海科在(微信)申请的渠道编号]
        /// </summary>
        public string ChannelNoWx { get; set; }

        /// <summary>
        /// 支付宝渠道号[服务商自行申请的支付宝渠道号(PID)]
        /// </summary>
        public string ChannelNoAli { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(PrivateKeyPassword))
            {
                PrivateKeyPassword = PrivateKeyPassword.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
