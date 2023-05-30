namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付方式表
    /// </summary>
    public class MchPayPassagePayWayDto : PayWayDto
    {
        /// <summary>
        /// 状态 0-停用 1-启用
        /// </summary>
        public byte PassageState { get; set; }

        /// <summary>
        /// 状态 0-未配置 1-已配置
        /// </summary>
        public byte IsConfig { get; set; }
    }
}
