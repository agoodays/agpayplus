namespace AGooday.AgPay.Components.Third.RQRS.Division
{
    /// <summary>
    /// 发起订单分账 响应参数
    /// </summary>
    public class PayOrderDivisionExecRS : AbstractRS
    {
        /// <summary>
        /// 分账状态 1-分账成功, 2-分账失败
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 上游分账批次号
        /// </summary>
        public string ChannelBatchOrderId { get; set; }

        /// <summary>
        /// 支付渠道错误码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 支付渠道错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}
