namespace AGooday.AgPay.Components.Third.RQRS.PayOrder
{
    /// <summary>
    /// 通用支付数据RQ
    /// </summary>
    public class CommonPayDataRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 请求参数： 支付数据包类型
        /// </summary>
        public string PayDataType { get; set; }
    }
}
