using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统用户认证表
    /// </summary>
    [Comment("系统用户认证表")]
    [Table("t_sys_user_auth")]
    public class SysUserAuth
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("auth_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long AuthId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Comment("用户ID")]
        [Required, Column("user_id", TypeName = "bigint")]
        public long UserId { get; set; }

        /// <summary>
        /// 登录类型  1-昵称 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博
        /// </summary>
        [Comment("登录类型  1-昵称 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博")]
        [Required, Column("identity_type", TypeName = "tinyint(6)")]
        public byte IdentityType { get; set; }

        /// <summary>
        /// 认证标识 ( 用户名 | open_id )
        /// </summary>
        [Comment(" 认证标识 ( 用户名 | open_id )")]
        [Required, Column("identifier", TypeName = "varchar(128)")]
        public string Identifier { get; set; }

        /// <summary>
        /// 密码凭证
        /// </summary>
        [Comment("密码凭证")]
        [Required, Column("credential", TypeName = "varchar(128)")]
        public string Credential { get; set; }

        /// <summary>
        /// salt
        /// </summary>
        [Comment("salt")]
        [Required, Column("salt", TypeName = "varchar(128)")]
        public string Salt { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统： MGR-运营平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }
    }
}
