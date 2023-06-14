using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 码牌模板信息表
    /// </summary>
    public class QrCodeShellQueryDto : PageQuery
    {
        /// <summary>
        /// 模板别名
        /// </summary>
        public string ShellAlias { get; set; }
    }
}
