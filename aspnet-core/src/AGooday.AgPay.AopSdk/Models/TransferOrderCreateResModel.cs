using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 转账下单响应实体类
    /// </summary>
    public class TransferOrderCreateResModel : AgPayObject
    {
        /// <summary>
        /// 转账单号(网关生成)
        /// </summary>
        [JsonProperty("transferId")]
        public string TransferId { get; set; }

        /// <summary>
        /// 商户发起的转账订单号
        /// </summary>
        [JsonProperty("mchOrderNo")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 订单转账金额
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        [JsonProperty("accountNo")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        [JsonProperty("bankName")]
        public string BankName { get; set; }

        /// <summary>
        /// 转账状态
        /// 0-订单生成
        /// 1-转账中
        /// 2-转账成功
        /// 3-转账失败
        /// 4-转账关闭
        /// </summary>
        [JsonProperty("state")]
        public int State { get; set; }

        /// <summary>
        /// 渠道转账单号
        /// </summary>
        [JsonProperty("channelOrderNo")]
        public string ChannelOrderNo { get; set; }

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
