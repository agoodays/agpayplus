using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 分账账号组
    /// </summary>
    public class MchDivisionReceiverGroupQueryDto : PageQuery
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        public string ReceiverGroupName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 自动分账组（当订单分账模式为自动分账，改组将完成分账逻辑） 0-否 1-是
        /// </summary>
        [BindNever]
        public byte AutoDivisionFlag { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [BindNever]
        public long CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [BindNever]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BindNever]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [BindNever]
        public DateTime UpdatedAt { get; set; }
    }
}
