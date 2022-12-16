using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 代理商信息表
    /// </summary>
    public class AgentInfoQueryDto : PageQuery
    {
        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 类型: 1-普通代理商, 2-特约代理商(服务商模式)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [BindNever]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [BindNever]
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [BindNever]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 代理商状态: 0-停用, 1-正常
        /// </summary>
        public byte? State { get; set; }
    }
}
