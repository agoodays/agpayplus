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
        /// 服务商编号
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// 交易私钥
        /// </summary>
        public string TradeKey { get; set; }

        /// <summary>
        /// 异步通知回调密钥
        /// </summary>
        public string NoticeKey { get; set; }

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
            if (!string.IsNullOrWhiteSpace(TradeKey))
            {
                TradeKey = TradeKey.Mask();
            }
            if (!string.IsNullOrWhiteSpace(NoticeKey))
            {
                NoticeKey = NoticeKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
