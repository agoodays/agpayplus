using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    [Table("t_mch_info")]
    public class MchInfo
    {
        /// <summary>
        /// 商户号
        /// </summary>
        [Key, Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [Required, Column("mch_name", TypeName = "varchar(64)")]
        public string MchName { get; set; }

        /// <summary>
        /// 商户简称
        /// </summary>
        [Required, Column("mch_short_name", TypeName = "varchar(32)")]
        public string MchShortName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [Required, Column("type", TypeName = "tinyint(6)")]
        public byte Type { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Column("contact_name", TypeName = "varchar(32)")]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [Column("contact_tel", TypeName = "varchar(32)")]
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [Column("contact_email", TypeName = "varchar(32)")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 商户状态: 0-停用, 1-正常
        /// </summary>
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 商户备注
        /// </summary>
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 初始用户ID（创建商户时，允许商户登录的用户）
        /// </summary>
        [Column("init_user_id", TypeName = "bigint")]
        public long? InitUserId { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

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
