using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统角色权限关联表 system role entitlement relate
    /// </summary>
    public class SysRoleEntRela
    {
        /**
         * 角色ID
         */
        public string RoleId;

        /**
         * 权限ID
         */
        public string EntId;
    }
}
