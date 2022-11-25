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
    }
}
