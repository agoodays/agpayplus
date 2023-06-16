using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.SxfPay
{
    /// <summary>
    /// 随行付 配置信息
    /// </summary>
    public class SxfPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte Sandbox { get; set; }
        /// <summary>
        /// 平台私钥
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 平台公钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 机构编号
        /// </summary>
        public string OrgId { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(PrivateKey))
            {
                PrivateKey = PrivateKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(PublicKey))
            {
                PublicKey = PublicKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
