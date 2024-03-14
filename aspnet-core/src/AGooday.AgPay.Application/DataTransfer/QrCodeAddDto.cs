using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 码牌信息表
    /// </summary>
    public class QrCodeAddDto : QrCodeDto
    {
        public int AddNum { get; set; }

        /// <summary>
        /// QrUrl 格式
        /// </summary>
        [BindNever]
        public string QrUrlFormat { get; set; }
    }
}
