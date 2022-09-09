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
    [Table("t_sys_user")]
    public class SysUser
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        [Key, Required, Column("sys_user_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long SysUserId { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        [Column("login_username")]
        public string LoginUsername { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Column("realname")]
        public string Realname { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column("telphone")]
        public string Telphone { get; set; }

        /// <summary>
        /// 性别 0-未知, 1-男, 2-女
        /// </summary>
        [Column("sex")]
        public byte Sex { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        [Column("avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        [Column("user_no")]
        public string UserNo { get; set; }

        /// <summary>
        /// 是否超管（超管拥有全部权限） 0-否 1-是
        /// </summary>
        [Column("is_admin")]
        public byte IsAdmin { get; set; }

        /// <summary>
        /// 状态 0-停用 1-启用
        /// </summary>
        [Column("state")]
        public byte State { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [Column("sys_type")]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 0(平台)
        /// </summary>
        [Column("belong_info_id")]
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
