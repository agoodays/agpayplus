using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 服务商信息表
    /// </summary>
    public class IsvInfoQueryDto : PageQuery
    {
        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 服务商名称
        /// </summary>
        public string IsvName { get; set; }

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
        /// 状态: 0-停用, 1-正常
        /// </summary>
        public byte? State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [BindNever]
        public string Remark { get; set; }
    }
}
