using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 分账账号组
    /// </summary>
    [Table("t_mch_division_receiver_group")]
    public class MchDivisionReceiverGroup
    {
        /// <summary>
        /// 组ID
        /// </summary>
        [Key, Required, Column("receiver_group_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Required, Column("receiver_group_name", TypeName = "varchar(64)")]
        public string ReceiverGroupName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 自动分账组（当订单分账模式为自动分账，改组将完成分账逻辑） 0-否 1-是
        /// </summary>
        [Required, Column("auto_division_flag", TypeName = "tinyint(6)")]
        public byte AutoDivisionFlag { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Required, Column("created_uid", TypeName = "bigint")]
        public long CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required, Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Required, Column("updated_at", TypeName = "timestamp")]
        public DateTime UpdatedAt { get; set; }
    }
}
