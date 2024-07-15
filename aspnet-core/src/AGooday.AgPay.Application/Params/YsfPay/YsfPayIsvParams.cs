using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.YsfPay
{
    /// <summary>
    /// 云闪付 配置信息
    /// </summary>
    public class YsfPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// serProvId
        /// </summary>
        public string SerProvId { get; set; }

        /// <summary>
        /// isvPrivateCertFile 证书
        /// </summary>
        public string IsvPrivateCertFile { get; set; }

        /// <summary>
        /// isvPrivateCertPwd
        /// </summary>
        public string IsvPrivateCertPwd { get; set; }

        /// <summary>
        /// ysfpayPublicKey
        /// </summary>
        public string YsfpayPublicKey { get; set; }

        /// <summary>
        /// acqOrgCodeList 支付机构号
        /// </summary>
        public string AcqOrgCode;

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(IsvPrivateCertPwd))
            {
                IsvPrivateCertPwd = IsvPrivateCertPwd.Mask();
            }
            if (!string.IsNullOrWhiteSpace(YsfpayPublicKey))
            {
                YsfpayPublicKey = YsfpayPublicKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
