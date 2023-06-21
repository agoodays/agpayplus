using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.LesPay
{
    /// <summary>
    /// 乐刷 配置信息
    /// </summary>
    public class LesPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte Sandbox { get; set; }
        /// <summary>
        /// 交易私钥
        /// </summary>
        public string TradeKey { get; set; }
        /// <summary>
        /// 服务商编号
        /// </summary>
        public string AgentId { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(TradeKey))
            {
                TradeKey = TradeKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
