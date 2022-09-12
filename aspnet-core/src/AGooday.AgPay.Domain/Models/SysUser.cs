using Microsoft.EntityFrameworkCore;
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
    /// 系统用户表
    /// </summary>
    [Comment("系统用户表")]
    [Table("t_sys_user")]
    public class SysUser
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        [Comment("系统用户ID")]
        [Key, Required, Column("sys_user_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long SysUserId { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        [Comment("登录用户名")]
        [Required, Column("login_username", TypeName = "varchar(32)")]
        public string LoginUsername { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Comment("真实姓名")]
        [Required, Column("realname", TypeName = "varchar(32)")]
        public string Realname { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Comment("手机号")]
        [Required, Column("telphone", TypeName = "varchar(32)")]
        public string Telphone { get; set; }

        /// <summary>
        /// 性别 0-未知, 1-男, 2-女
        /// </summary>
        [Comment("性别 0-未知, 1-男, 2-女")]
        [Required, Column("sex", TypeName = "tinyint(6)")]
        public byte Sex { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        [Comment("头像地址")]
        [Column("avatar_url", TypeName = "varchar(128)")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        [Comment("员工编号")]
        [Column("user_no", TypeName = "varchar(32)")]
        public string UserNo { get; set; }

        /// <summary>
        /// 是否超管（超管拥有全部权限） 0-否 1-是
        /// </summary>
        [Comment("是否超管（超管拥有全部权限） 0-否 1-是")]
        [Required, Column("is_admin", TypeName = "tinyint(6)")]
        public byte IsAdmin { get; set; }

        /// <summary>
        /// 状态 0-停用 1-启用
        /// </summary>
        [Comment("状态 0-停用 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统： MGR-运营平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 0(平台)
        /// </summary>
        [Comment("所属商户ID / 0(平台)")]
        [Required, Column("belong_info_id", TypeName = "varchar(64)")]
        public string BelongInfoId { get; set; }

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
