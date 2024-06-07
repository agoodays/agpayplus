namespace AGooday.AgPay.Payment.Api.Channel.YsePay.Enumerator
{
    public class YsePayEnum
    {
        public enum TradeStatus
        {
            /// <summary>
            /// 交易创建，等待买家付款。
            /// </summary>
            WAIT_BUYER_PAY,
            /// <summary>
            /// 在指定时间段内未支付时关闭的交易；客户主动关闭订单。
            /// </summary>
            TRADE_CLOSED,
            /// <summary>
            /// 交易成功，且可对该交易做操作，如：多级分润、退款等。
            /// </summary>
            TRADE_SUCCESS,
            /// <summary>
            /// 部分退款成功。
            /// </summary>
            TRADE_PART_REFUND,
            /// <summary>
            /// 全部退款成功。
            /// </summary>
            TRADE_ALL_REFUND,
            /// <summary>
            /// 交易正在处理中，可对该交易做查询，避免重复支付。
            /// </summary>
            TRADE_PROCESS,  
            /// <summary>
            /// 交易失败
            /// </summary>
            TRADE_FAILD,
            /// <summary>
            /// 买家已付款，等待卖家发货
            /// </summary>
            WAIT_SELLER_SEND_GOODS,
            /// <summary>
            /// 卖家已发货，等待买家确认
            /// </summary>
            WAIT_BUYER_CONFIRM_GOODS,
            /// <summary>
            /// 支付中, 该状态为未知状态，请勿当成失败状态处理，请等待支付通知或继续查询
            /// </summary>
            TRADE_ABNORMALITY,
            /// <summary>
            /// 交易失败
            /// </summary>
            TRADE_FAILED,
        }

        public static TradeStatus ConvertTradeStatus(string tradeStatus)
        {
            Enum.TryParse(tradeStatus, out TradeStatus _tradeStatus);
            return _tradeStatus;
        }

        /// <summary>
        /// 退款交易状态
        /// </summary>
        public enum RefundState {
            /// <summary>
            /// 成功
            /// </summary>
            success,
            /// <summary>
            /// 处理中
            /// </summary>
            in_process,
            /// <summary>
            /// 失败
            /// </summary>
            fail,
        }

        public static RefundState ConvertRefundState(string refundState)
        {
            Enum.TryParse(refundState, out RefundState _refundState);
            return _refundState;
        }
    }
}
