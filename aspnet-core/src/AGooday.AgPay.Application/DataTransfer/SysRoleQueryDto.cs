using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统角色表
    /// </summary>
    public class SysRoleQueryDto : PageQuery
    {
        /// <summary>
        /// 角色ID, ROLE_开头
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [BindNever]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 0(平台)
        /// </summary>
        [BindNever]
        public string BelongInfoId { get; set; }
    }
}
