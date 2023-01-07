using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 用户团队信息表
    /// </summary>
    [Comment("用户团队信息表")]
    [Table("t_sys_user_team")]
    public class SysUserTeam
    {
        /// <summary>
        /// 团队ID
        /// </summary>
        [Comment("团队ID")]
        [Key, Required, Column("team_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long TeamId { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        [Comment("团队名称")]
        [Required, Column("team_name", TypeName = "varchar(32)")]
        public string TeamName { get; set; }

        /// <summary>
        /// 团队编号
        /// </summary>
        [Comment("团队编号")]
        [Required, Column("team_no", TypeName = "varchar(64)")]
        public string TeamNo { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 统计周期: year-年, quarter-季度, month-月, week-周
        /// </summary>
        [Comment("统计周期: year-年, quarter-季度, month-月, week-周")]
        [Required, Column("stat_range_type", TypeName = "varchar(20)")]
        public string StatRangeType { get; set; }

        /// <summary>
        /// 所属商户ID / 所属代理商ID / 0(平台)
        /// </summary>
        [Comment("所属商户ID / 所属代理商ID / 0(平台)")]
        [Required, Column("belong_info_id", TypeName = "varchar(64)")]
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

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
