using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using AGooday.AgPay.Domain.Core.Tracker;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 服务商信息表
    /// </summary>
    [Comment("服务商信息表")]
    [Table("t_isv_info")]
    public class IsvInfo : AbstractTrackableTimestamps, ITrackableUser
    {
        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Key, Required, Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 服务商名称
        /// </summary>
        [Comment("服务商名称")]
        [Required, Column("isv_name", TypeName = "varchar(64)")]
        public string IsvName { get; set; }

        /// <summary>
        /// 服务商简称
        /// </summary>
        [Comment("服务商简称")]
        [Required, Column("isv_short_name", TypeName = "varchar(32)")]
        public string IsvShortName { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Comment("联系人姓名")]
        [Column("contact_name", TypeName = "varchar(32)")]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [Comment("联系人手机号")]
        [Column("contact_tel", TypeName = "varchar(32)")]
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [Comment("联系人邮箱")]
        [Column("contact_email", TypeName = "varchar(32)")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-正常
        /// </summary>
        [Comment("状态: 0-停用, 1-正常")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint(20)")]
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }
    }
}
