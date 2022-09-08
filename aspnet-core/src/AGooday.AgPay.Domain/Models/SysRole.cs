using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统角色表
    /// </summary>
    public class SysRole
    {
        /**
         * 角色ID, ROLE_开头
         */
        public string RoleId { get; set; }

        /**
         * 角色名称
         */
        public string RoleName { get; set; }

        /**
         * 所属系统： MGR-运营平台, MCH-商户中心
         */
        public string SysType { get; set; }

        /**
         * 所属商户ID / 0(平台)
         */
        public string BelongInfoId { get; set; }

        /**
         * 更新时间
         */
        public DateTime UpdatedAt { get; set; }
    }
}
