using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Models;
using MediatR;

namespace AGooday.AgPay.Domain.Queries.SysUsers
{
    public class SysUserQuery : PageQuery, IRequest<IEnumerable<(SysUser SysUser, SysUserTeam SysUserTeam)>>
    {
        /// <summary>
        /// 当前用户ID
        /// </summary>
        public long? CurrentUserId { get; set; }

        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long? SysUserId { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Realname { get; set; }

        /// <summary>
        /// 用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员
        /// </summary>
        public byte? UserType { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
        /// </summary>
        public string BelongInfoId { get; set; }
    }
}
