namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    public class CommonPayDataRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 请求参数： 支付数据包类型
        /// </summary>
        public string PayDataType { get; set; }
    }
}
