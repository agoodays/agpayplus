using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using AGooday.AgPay.Domain.Core.Tracker;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户应用表
    /// </summary>
    [Table("t_mch_app")]
    [Comment("商户应用表")]
    public class MchApp : AbstractTrackableTimestamps, ITrackableUser
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID")]
        [Key, Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [Comment("应用名称")]
        [Required, Column("app_name", TypeName = "varchar(64)")]
        public string AppName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 应用状态: 0-停用, 1-正常
        /// </summary>
        [Comment("应用状态: 0-停用, 1-正常")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 是否默认: 0-否, 1-是
        /// </summary>
        [Comment("是否默认: 0-否, 1-是")]
        [Required, Column("default_flag", TypeName = "tinyint(6)")]
        public byte DefaultFlag { get; set; }

        /// <summary>
        /// 支持的签名方式 ["MD5", "RSA2"]
        /// </summary>
        [Comment("支持的签名方式 [\"MD5\", \"RSA2\"]")]
        [Column("app_sign_type", TypeName = "json")]
        public string AppSignType { get; set; }

        /// <summary>
        /// 应用私钥
        /// </summary>
        [Comment("应用私钥")]
        [Required, Column("app_secret", TypeName = "varchar(128)")]
        public string AppSecret { get; set; }

        /// <summary>
        /// RSA2应用公钥
        /// </summary>
        [Comment("RSA2应用公钥")]
        [Column("app_rsa2_public_key", TypeName = "varchar(256)")]
        public string AppRsa2PublicKey { get; set; }

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
