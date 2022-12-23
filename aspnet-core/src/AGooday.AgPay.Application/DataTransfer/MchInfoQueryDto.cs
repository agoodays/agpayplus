using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    public class MchInfoQueryDto : PageQuery
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）
        /// </summary>
        public string MchLevel { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

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
        /// 商户状态: 0-停用, 1-正常
        /// </summary>
        public byte? State { get; set; }
    }
}
