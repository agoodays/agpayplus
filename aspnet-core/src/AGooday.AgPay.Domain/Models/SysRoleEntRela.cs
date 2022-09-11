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
    /// 系统角色权限关联表 system role entitlement relate
    /// </summary>
    [Table("t_sys_role_ent_rela")]
    public class SysRoleEntRela
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required, Column("role_id", TypeName = "varchar(32)")]
        public string RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [Required, Column("ent_id", TypeName = "varchar(64)")]
        public string EntId { get; set; }
    }
}
