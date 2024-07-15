using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.LklPay
{
    /// <summary>
    /// 拉卡拉 配置信息
    /// </summary>
    public class LklPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// 机构号
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 证书序列号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 应用私钥证书（.pem格式）
        /// </summary>
        public string PrivateCert { get; set; }

        /// <summary>
        /// 拉卡拉公钥证书（.cer格式）
        /// </summary>
        public string PublicCert { get; set; }

        /// <summary>
        /// 微信渠道号[服务商通过海科在(微信)申请的渠道编号]
        /// </summary>
        public string ChannelNoWx { get; set; }

        /// <summary>
        /// 支付宝渠道号[服务商自行申请的支付宝渠道号(PID)]
        /// </summary>
        public string ChannelNoAli { get; set; }

        /// <summary>
        /// 微信渠道拓展二维码URL
        /// </summary>
        public string WxOpenUrl { get; set; }

        /// <summary>
        /// 支付宝渠道拓展二维码URL
        /// </summary>
        public string AliChannelExtUrl { get; set; }

        public override string DeSenData()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
