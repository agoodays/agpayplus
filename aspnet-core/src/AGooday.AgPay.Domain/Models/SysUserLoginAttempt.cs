using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统用户登录尝试记录表
    /// </summary>
    [Comment("系统用户登录尝试记录表")]
    [Table("t_sys_user_login_attempt")]
    public class SysUserLoginAttempt
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long Id { get; set; }

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
        /// IP地址
        /// </summary>
        [Comment("IP地址")]
        [Required, Column("ip_address", TypeName = "varchar(128)")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 登录成功
        /// </summary>
        [Comment("登录成功")]
        [Required, Column("success", TypeName = "tinyint(1)")]
        public bool Success { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 尝试时间
        /// </summary>
        [Comment("尝试时间")]
        [Required, Column("attempt_time", TypeName = "timestamp(6)")]
        public DateTime AttemptTime { get; set; }
    }
}
