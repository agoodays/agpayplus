namespace AGooday.AgPay.Application.DataTransfer
{
    public class PayOrderProfitDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 运营平台, 代理商名称
        /// </summary>
        public string InfoName { get; set; }

        /// <summary>
        /// PLATFORM-运营平台, AGENT-代理商
        /// </summary>
        public string InfoType { get; set; }

        /// <summary>
        /// 支付订单号（与t_pay_order对应）
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 费率快照
        /// </summary>
        public decimal FeeRate { get; set; }

        /// <summary>
        /// 费率快照描述
        /// </summary>
        public string FeeRateDesc { get; set; }

        /// <summary>
        /// 分润金额(实际分润),单位分
        /// </summary>
        public long ProfitAmount { get; set; }

        /// <summary>
        /// 收单分润金额,单位分
        /// </summary>
        public long OrderProfitAmount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
