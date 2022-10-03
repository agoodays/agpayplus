using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 分账响应结果
    /// </summary>
    public class PayOrderDivisionExecResModel : AgPayObject
    {
        /// <summary>
        /// 分账状态 1-分账成功, 2-分账失败
        /// </summary>
        [JsonProperty("state")]
        public byte State { get; set; }

        /// <summary>
        /// 上游分账批次号
        /// </summary>
        [JsonProperty("channelBatchOrderId")]
        public string ChannelBatchOrderId { get; set; }

        /// <summary>
        /// 支付渠道错误码
        /// </summary>
        [JsonProperty("errCode")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 支付渠道错误信息
        /// </summary>
        [JsonProperty("errMsg")]
        public string ErrMsg { get; set; }
    }
}
