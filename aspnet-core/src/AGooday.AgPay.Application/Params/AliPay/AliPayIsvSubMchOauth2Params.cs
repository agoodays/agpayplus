using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.AliPay
{
    /// <summary>
    /// 支付宝 oauth2 配置参数
    /// </summary>
    public class AliPayIsvSubMchOauth2Params : IsvSubMchOauth2Params
    {
        /// <summary>
        /// 特约商户小程序支付跳转的选择
        /// </summary>
        public byte? IsUseSubmchAccount { get; set; }

        /// <summary>
        /// 小程序参数配置
        /// </summary>
        public AliLiteParams LiteParams { get; set; }

        public override string DeSenData()
        {
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
