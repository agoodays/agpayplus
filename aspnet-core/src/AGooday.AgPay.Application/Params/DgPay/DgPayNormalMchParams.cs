using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.DgPay
{
    /// <summary>
    /// 斗拱 普通商户参数定义
    /// </summary>
    public class DgPayNormalMchParams : NormalMchParams
    {
        /// <summary>
        /// 汇付客户Id
        /// </summary>
        public string HuifuId { get; set; }

        /// <summary>
        /// 分配的产品号
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 商户私钥
        /// </summary>
        public string RsaPrivateKey { get; set; }

        /// <summary>
        /// 斗拱公钥
        /// </summary>
        public string RsaPublicKey { get; set; }

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
