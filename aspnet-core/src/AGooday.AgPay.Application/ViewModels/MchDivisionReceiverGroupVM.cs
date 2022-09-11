using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    /// <summary>
    /// 分账账号组
    /// </summary>
    public class MchDivisionReceiverGroupVM
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
        public byte AutoDivisionFlag { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public long CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
