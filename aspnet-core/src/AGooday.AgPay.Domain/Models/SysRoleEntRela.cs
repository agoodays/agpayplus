using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统角色权限关联表 system role entitlement relate
    /// </summary>
    [Comment("系统角色权限关联表")]
    [Table("t_sys_role_ent_rela")]
    public class SysRoleEntRela
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Comment("角色ID")]
        [Required, Column("role_id", TypeName = "varchar(32)")]
        public string RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [Comment("权限ID")]
        [Required, Column("ent_id", TypeName = "varchar(64)")]
        public string EntId { get; set; }
    }
}
