using AGooday.AgPay.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 操作员<->角色 关联表
    /// </summary>
    public class SysUserRoleRelaQueryDto : PageQuery
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }
    }
}
