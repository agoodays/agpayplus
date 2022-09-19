using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统角色权限关联表 system role entitlement relate
    /// </summary>
    public class SysRoleEntRelaDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public string EntId { get; set; }
    }
}
