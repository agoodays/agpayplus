using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 操作员<->角色 关联表
    /// </summary>
    public class SysUserRoleRela
    {
        /**
         * 用户ID
         */
        private long UserId;

        /**
         * 角色ID
         */
        private string RoleId;
    }
}
