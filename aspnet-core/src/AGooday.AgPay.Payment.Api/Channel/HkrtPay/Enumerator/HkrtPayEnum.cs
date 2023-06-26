using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.Channel.HkrtPay.Enumerator
{
    public class HkrtPayEnum
    {
        /// <summary>
        /// 支付方式
        /// 微信支付:WX 支付宝支付:ALI 银联二维码支付:UNIONQR
        /// </summary>
        public enum TradeType
        {
            /// <summary>
            /// 微信支付
            /// </summary>
            WX,
            /// <summary>
            /// 支付宝支付
            /// </summary>
            ALI,
            /// <summary>
            /// 银联二维码支付
            /// </summary>
            UNIONQR
        }

        /// <summary>
        /// 交易状态
        /// 1：交易成功
        /// 2：交易失败
        /// 3：交易进行中
        /// 4：交易超时
        /// </summary>
        public enum TradeStatus
        {
            /// <summary>
            /// 交易成功
            /// </summary>
            Success = 1,

            /// <summary>
            /// 交易失败
            /// </summary>
            Failed = 2,

            /// <summary>
            /// 交易进行中
            /// </summary>
            Paying = 3,

            /// <summary>
            /// 交易超时
            /// </summary>
            Timeout = 4,
        }

        /// <summary>	
        /// 退款结果:
        /// 1：成功（退款申请接收成功，退款的到账时间以实际为准）；
        /// 2：失败（退款申请失败，错误原因参考status_msg）；
        /// 3：结果未知（退款申请处理结果未知,请调用退款查询接口获取退款结果状态）
        /// 4：交易超时
        /// </summary>
        public enum RefundStatus
        {
            /// <summary>
            /// 成功（退款申请接收成功，退款的到账时间以实际为准）；
            /// </summary>
            Success = 1,

            /// <summary>
            /// 失败（退款申请失败，错误原因参考status_msg）；
            /// </summary>
            Failed = 2,

            /// <summary>
            /// 结果未知（退款申请处理结果未知,请调用退款查询接口获取退款结果状态）
            /// </summary>
            Refunding = 3,

            /// <summary>
            /// 交易超时
            /// </summary>
            Timeout = 4,
        }

        public static TradeStatus ConvertTradeStatus(string status)
        {
            Enum.TryParse(status, out TradeStatus tradeStatus);
            return tradeStatus;
        }
        public static RefundStatus ConvertRefundStatus(string status)
        {
            Enum.TryParse(status, out RefundStatus refundStatus);
            return refundStatus;
        }

        public static TradeType ConvertTradeType(string type)
        {
            Enum.TryParse(type, out TradeType tradeType);
            return tradeType;
        }

        /// <summary>
        /// 支付方式
        /// 微信支付:WX 支付宝支付:ALI 银联二维码支付:UNIONQR
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetTradeType(string wayCode)
        {
            string payType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payType = TradeType.ALI.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = TradeType.WX.ToString();
                    break;
                case CS.PAY_WAY_CODE.UP_QR:
                    payType = TradeType.UNIONQR.ToString();
                    break;
                default:
                    break;
            }
            return payType;
        }
    }
}
