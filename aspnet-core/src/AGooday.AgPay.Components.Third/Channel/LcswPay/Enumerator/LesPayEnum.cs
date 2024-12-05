using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.Channel.LcswPay.Enumerator
{
    public class LcswPayEnum
    {

        /// <summary>
        /// 支付方式，
        /// 010 微信，
        /// 020 支付宝，
        /// 060 qq钱包，
        /// 100 翼支付，
        /// 110 银联二维码，
        /// 000 自动识别类型
        /// </summary>
        public interface PayWay
        {
            /// <summary>
            /// 自动识别类型
            /// </summary>
            public const string AUTO = "000";
            /// <summary>
            /// 微信
            /// </summary>
            public const string WECHAT = "010";
            /// <summary>
            /// 支付宝
            /// </summary>
            public const string ALIPAY = "020";
            /// <summary>
            /// qq钱包
            /// </summary>
            public const string QQPAY = "060";
            /// <summary>
            /// 翼支付
            /// </summary>
            public const string BESTPAY = "100";
            /// <summary>
            /// 银联二维码
            /// </summary>
            public const string UNIONPAY = "110";
        }

        public enum TradeState
        {
            /// <summary>
            /// 支付成功/退款成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 转入退款
            /// </summary>
            REFUND,
            /// <summary>
            /// 未支付
            /// </summary>
            NOTPAY,
            /// <summary>
            /// 已关闭
            /// </summary>
            CLOSED,
            /// <summary>
            /// 用户支付中
            /// </summary>
            USERPAYING,
            /// <summary>
            /// 已撤销
            /// </summary>
            REVOKED,
            /// <summary>
            /// 未支付支付超时
            /// </summary>
            NOPAY,
            /// <summary>
            /// 支付失败
            /// </summary>
            PAYERROR,
            /// <summary>
            /// 退款失败
            /// </summary>
            FAIL,
            /// <summary>
            /// 退款中
            /// </summary>
            REFUNDING,
            /// <summary>
            /// 退款超时
            /// </summary>
            NOREFUND,
        }

        public static TradeState ConvertTradeState(string tradeState)
        {
            Enum.TryParse(tradeState, out TradeState _tradeState);
            return _tradeState;
        }

        /// <summary>
        /// 支付方式，
        /// 010 微信，
        /// 020 支付宝，
        /// 060 qq钱包，
        /// 100 翼支付，
        /// 110 银联二维码，
        /// 000 自动识别类型
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetPayType(string wayCode)
        {
            string payType = PayWay.AUTO;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_BAR:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_APP:
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payType = PayWay.ALIPAY;
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = PayWay.WECHAT;
                    break;
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = PayWay.UNIONPAY;
                    break;
                default:
                    break;
            }
            return payType;
        }
    }
}
