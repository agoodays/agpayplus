using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 用户团队信息表
    /// </summary>
    public class SysUserTeamQueryDto : PageQuery
    {
        /// <summary>
        /// 团队ID
        /// </summary>
        public long TeamId { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 团队编号
        /// </summary>
        public string TeamNo { get; set; }

        /// <summary>
        /// 所属代理商/商户
        /// </summary>
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }
    }
}
