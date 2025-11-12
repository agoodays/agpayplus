using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Models;

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
        public long? StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }
    }

    public class MchStoreQueryResult
    {
        public MchStore MchStore { get; set; }
        public MchInfo MchInfo { get; set; }
    }
}
