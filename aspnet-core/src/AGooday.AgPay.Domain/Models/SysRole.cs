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
    /// 系统角色表
    /// </summary>
    [Table("t_sys_role")]
    public class SysRole
    {
        /// <summary>
        /// 角色ID, ROLE_开头
        /// </summary>
        [Key, Required, Column("role_id", TypeName = "varchar(32)")]
        public string RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required, Column("role_name", TypeName = "varchar(32)")]
        public string RoleName { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 0(平台)
        /// </summary>
        [Required, Column("belong_info_id", TypeName = "varchar(64)")]
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Required, Column("updated_at", TypeName = "timestamp")]
        public DateTime UpdatedAt { get; set; }
    }
}
