using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户门店表
    /// </summary>
    public class MchStoreQueryDto : PageQuery
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }
    }
}
