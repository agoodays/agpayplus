using AGooday.AgPay.Common.Models;
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
    public class SysRoleEntRelaQueryDto : PageQuery
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }
    }
}
