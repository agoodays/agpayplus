namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付费率配置表
    /// </summary>
    public class PayRateConfigDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 配置类型: ISVCOST-服务商低价, AGENTRATE-代理商费率, AGENTDEF-代理商默认费率, MCHAPPLYDEF-商户进件默认费率, MCHRATE-商户费率
        /// </summary>
        public string ConfigType { get; set; }

        /// <summary>
        /// 账号类型:ISV-服务商, ISV_OAUTH2-服务商oauth2, AGENT-代理商, MCH_APP-商户应用, MCH_APP_OAUTH2-商户应用oauth2
        /// </summary>
        public string InfoType { get; set; }

        /// <summary>
        /// 服务商号/代理商号/商户号/应用ID
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 费率类型:SINGLE-单笔费率, LEVEL-阶梯费率
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 阶梯模式: 模式: NORMAL-普通模式, UNIONPAY-银联模式
        /// </summary>
        public string LevelMode { get; set; }

        /// <summary>
        /// 支付方式费率
        /// </summary>
        public decimal? FeeRate { get; set; }

        /// <summary>
        /// 是否支持进件: 0-不支持, 1-支持
        /// </summary>
        public byte ApplymentSupport { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public List<PayRateLevelConfigDto> PayRateLevelConfigs { get; set; }
    }
}
