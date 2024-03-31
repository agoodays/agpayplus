using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.DgPay
{
    /// <summary>
    /// 斗拱 配置信息
    /// </summary>
    public class DgPayIsvParams : IsvParams
    {
        /// <summary>
        /// 商户结算周期
        /// </summary>
        public string SettleCycle { get; set; }

        /// <summary>
        /// D1结算费率（填写值为 0.00-100.00 之间）
        /// </summary>
        public decimal SettleFee { get; set; }

        /// <summary>
        /// 商户手动取现
        /// </summary>
        public string MchSettManual { get; set; }

        /// <summary>
        /// 取现费率（填写值为 0.00-100.00 之间）
        /// </summary>
        public decimal CashFee { get; set; }

        /// <summary>
        /// 分配的产品号
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 分配的系统号
        /// </summary>
        public string SysId { get; set; }

        /// <summary>
        /// 微信渠道拓展二维码URL
        /// </summary>
        public string WxOpenUrl { get; set; }

        /// <summary>
        /// 支付宝渠道拓展二维码URL
        /// </summary>
        public string AliChannelExtUrl { get; set; }

        /// <summary>
        /// 【电子协议】协议模板号
        /// </summary>
        public string AgreementModel { get; set; }

        /// <summary>
        /// 【电子协议】协议模板名称
        /// </summary>
        public string AgreementName { get; set; }

        /// <summary>
        /// 商户私钥
        /// </summary>
        public string RsaPrivateKey { get; set; }

        /// <summary>
        /// 斗拱公钥
        /// </summary>
        public string RsaPublicKey { get; set; }

        /// <summary>
        /// webhook终端秘钥（智能POS需配置此项）
        /// </summary>
        public string WebhookPrivateKey { get; set; }

        /// <summary>
        /// 智能POS公钥（智能POS需配置此项）
        /// </summary>
        public string PosPublicKey { get; set; }

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
            if (!string.IsNullOrWhiteSpace(WebhookPrivateKey))
            {
                WebhookPrivateKey = WebhookPrivateKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(PosPublicKey))
            {
                PosPublicKey = PosPublicKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
