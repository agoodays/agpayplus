using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统角色表
    /// </summary>
    public class SysRoleCreateDto
    {
        /// <summary>
        /// 角色ID, ROLE_开头
        /// </summary>
        [BindNever]
        public string RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        [BindNever]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
        /// </summary>
        [BindNever]
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [BindNever]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 权限信息集合
        /// </summary>
        public List<string> EntIds { get; set; }
    }
}
