using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Models
{
    /// <summary>
    /// 发起分账
    /// </summary>
    public class PayOrderDivisionExecReqModel : AgPayObject
    {
        /// <summary>
        /// 商户号
        /// </summary>
        [JsonProperty("mchNo")]
        public string mchNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [JsonProperty("appId")]
        public string appId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [JsonProperty("mchOrderNo")]
        public string mchOrderNo { get; set; }

        /// <summary>
        /// 支付系统订单号
        /// </summary>
        [JsonProperty("payOrderId")]
        public string payOrderId { get; set; }

        /// <summary>
        /// 是否使用系统配置的自动分账组： 0-否 1-是
        /// </summary>
        [JsonProperty("useSysAutoDivisionReceivers")]
        public byte useSysAutoDivisionReceivers { get; set; }

        /// <summary>
        ///  接收者账号列表（JSONArray 转换为字符串类型）
        /// 仅当useSysAutoDivisionReceivers=0 时有效。
        /// 
        /// 参考：
        /// 
        /// 方式1： 按账号纬度
        /// [{
        ///     receiverId: 800001,
        ///     divisionProfit: 0.1 (若不填入则使用系统默认配置值)
        /// }]
        /// 
        /// 方式2： 按组纬度
        /// [{
        ///     receiverGroupId: 100001, (该组所有 当前订单的渠道账号并且可用状态的全部参与分账)
        ///     divisionProfit: 0.1 (每个账号的分账比例， 若不填入则使用系统默认配置值， 建议不填写)
        /// }]
        /// </summary>
        [JsonProperty("receivers")]
        public string receivers { get; set; }
    }
}
