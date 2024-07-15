using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.JlPay
{
    /// <summary>
    /// 嘉联 配置信息
    /// </summary>
    public class JlPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// 机构编号
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 微信渠道拓展二维码URL
        /// </summary>
        public string WxOpenUrl { get; set; }

        /// <summary>
        /// 支付宝渠道拓展二维码URL
        /// </summary>
        public string AliChannelExtUrl { get; set; }

        /// <summary>
        /// 商户私钥
        /// </summary>
        public string RsaPrivateKey { get; set; }

        /// <summary>
        /// 嘉联公钥
        /// </summary>
        public string RsaPublicKey { get; set; }

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
            if (!string.IsNullOrWhiteSpace(RsaPrivateKey))
            {
                RsaPrivateKey = RsaPrivateKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(RsaPublicKey))
            {
                RsaPublicKey = RsaPublicKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
