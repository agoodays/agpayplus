using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户分账接收者账号绑定关系表
    /// </summary>
    public class MchDivisionReceiverModifyDto
    {
        /// <summary>
        /// 分账接收者ID
        /// </summary>
        public long? ReceiverId { get; set; }

        /// <summary>
        /// 接收者账号别名
        /// </summary>
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 组ID（便于商户接口使用）
        /// </summary>
        public long? ReceiverGroupId { get; set; }

        /// <summary>
        /// 分账比例
        /// </summary>
        public decimal? DivisionProfit { get; set; }

        /// <summary>
        /// 分账状态（本系统状态，并不调用上游关联关系）: 1-正常分账, 0-暂停分账
        /// </summary>
        public byte State { get; set; }
    }
}
