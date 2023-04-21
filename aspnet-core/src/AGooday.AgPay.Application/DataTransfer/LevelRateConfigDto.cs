namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 阶梯费率信息表
    /// </summary>
    public class LevelRateConfigDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 支付费率配置ID
        /// </summary>
        public long RateConfigId { get; set; }

        /// <summary>
        /// 银行卡类型: DEBIT-借记卡（储蓄卡）, CREDIT-贷记卡（信用卡）
        /// </summary>
        public string BankCardType { get; set; }

        /// <summary>
        /// 最小金额: 计算时大于此值
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// 最大金额: 计算时小于或等于此值
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// 保底费用
        /// </summary>
        public int MinFee { get; set; }

        /// <summary>
        /// 封顶费用
        /// </summary>
        public int MaxFee { get; set; }

        /// <summary>
        /// 支付方式费率
        /// </summary>
        public decimal? FeeRate { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

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
