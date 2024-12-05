using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 分账账号组
    /// </summary>
    [Comment("分账账号组")]
    [Table("t_mch_division_receiver_group")]
    public class MchDivisionReceiverGroup
    {
        /// <summary>
        /// 组ID
        /// </summary>
        [Comment("组ID")]
        [Key, Required, Column("receiver_group_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Comment("组名称")]
        [Required, Column("receiver_group_name", TypeName = "varchar(64)")]
        public string ReceiverGroupName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 自动分账组（当订单分账模式为自动分账，改组将完成分账逻辑） 0-否 1-是
        /// </summary>
        [Comment("自动分账组（当订单分账模式为自动分账，改组将完成分账逻辑） 0-否 1-是")]
        [Required, Column("auto_division_flag", TypeName = "tinyint(6)")]
        public byte AutoDivisionFlag { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Required, Column("created_uid", TypeName = "bigint")]
        public long CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}
