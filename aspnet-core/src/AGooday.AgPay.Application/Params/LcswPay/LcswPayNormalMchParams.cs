using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.LcswPay
{
    public class LcswPayNormalMchParams : NormalMchParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// 令牌标识
        /// </summary>
        public string AccessToken { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                AccessToken = AccessToken.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
