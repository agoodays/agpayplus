using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户应用表
    /// </summary>
    public class MchAppQueryDto : PageQuery
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 应用状态: 0-停用, 1-正常
        /// </summary>
        public byte? State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [BindNever]
        public string Remark { get; set; }
    }
}
