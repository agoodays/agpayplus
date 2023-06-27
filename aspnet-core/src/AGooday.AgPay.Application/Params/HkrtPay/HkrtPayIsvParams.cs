using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.HkrtPay
{
    /// <summary>
    /// 海科融通 配置信息
    /// </summary>
    public class HkrtPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte Sandbox { get; set; }

        /// <summary>
        /// 服务商编号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 接入SaaS平台的服务商标识，API接口使用。
        /// </summary>
        public string AccessId { get; set; }

        /// <summary>
        /// 服务商的接入秘钥，API接口签名使用。
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 服务商的传输密钥
        /// </summary>
        public string TransferKey { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(AccessKey))
            {
                AccessKey = AccessKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(TransferKey))
            {
                TransferKey = TransferKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
