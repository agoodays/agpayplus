using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 统计查询
    /// </summary>
    public class StatisticQueryDto : DatePageQuery
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
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 服务商名称
        /// </summary>
        public string IsvName { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public long? StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他
        /// </summary>
        public string WayType { get; set; }

        public string QueryDateType { get; set; }

        public string Method { get; set; }
    }
}
