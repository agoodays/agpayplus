namespace AGooday.AgPay.Payment.Api.RQRS.Division
{
    public class PayOrderDivisionExecRS : AbstractRS
    {
        /// <summary>
        /// 分账状态 1-分账成功, 2-分账失败
        /// </summary>
        private byte State { get; set; }

        /// <summary>
        /// 上游分账批次号
        /// </summary>
        private string ChannelBatchOrderId { get; set; }

        /// <summary>
        /// 支付渠道错误码
        /// </summary>
        private string ErrCode { get; set; }

        /// <summary>
        /// 支付渠道错误信息
        /// </summary>
        private string ErrMsg { get; set; }
    }
}
