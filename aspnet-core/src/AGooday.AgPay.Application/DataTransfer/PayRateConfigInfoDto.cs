namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付费率配置表
    /// </summary>
    public class PayRateConfigInfoDto : PayRateConfigDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string InfoName { get; set; }

        /// <summary>
        /// 代理商等级: 1-一级, 2-二级, 3-三级 ...
        /// </summary>
        public byte? AgentLevel { get; set; }

        /// <summary>
        /// 支付方式费率描述
        /// </summary>
        public string FeeRateDesc { get; set; }
    }
}
