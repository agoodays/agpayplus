using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.Channel.AllinPay.Enumerator
{
    public class AllinPayEnum
    {
        public enum PayType
        {
            /// <summary>
            /// 微信扫码支付
            /// </summary>
            W01,
            /// <summary>
            /// 微信JS支付
            /// </summary>
            W02,
            /// <summary>
            /// 微信APP支付
            /// </summary>
            W03,
            /// <summary>
            /// 微信小程序支付
            /// </summary>
            W06,
            /// <summary>
            /// 支付宝扫码支付
            /// </summary>
            A01,
            /// <summary>
            /// 支付宝JS支付
            /// </summary>
            A02,
            /// <summary>
            /// 支付宝APP支付
            /// </summary>
            A03,
            /// <summary>
            /// 银联扫码支付(CSB)
            /// </summary>
            U01,
            /// <summary>
            /// 银联JS支付
            /// </summary>
            U02,
            /// <summary>
            /// 数币扫码支付
            /// </summary>
            S01,
            /// <summary>
            /// 数字货币H5
            /// </summary>
            S03,
            /// <summary>
            /// 网联支付
            /// </summary>
            N03
        }

        /// <summary>
        /// 支付方式，枚举值
        /// 取值范围：
        /// 00 主扫
        /// 01 被扫
        /// 02 公众号/服务窗
        /// 03 小程序
        /// </summary>
        public interface PayWay
        {
            /// <summary>
            /// 被扫
            /// </summary>
            public const string BAR = "00";
            /// <summary>
            /// 主扫
            /// </summary>
            public const string QR = "01";
            /// <summary>
            /// 公众号/服务窗
            /// </summary>
            public const string JSAPI = "02";
            /// <summary>
            /// 小程序
            /// </summary>
            public const string LITE = "03";
        }

        /// <summary>
        /// 订单状态，枚举值
        /// 取值范围：
        /// SUCCESS 交易成功
        /// FAIL 交易失败
        /// PAYING 支付中
        /// CLOSED 已关闭
        /// CANCELED 已撤销
        /// 
        /// 退款状态，枚举值
        /// 取值范围：
        /// REFUNDSUC 退款成功
        /// REFUNDFAIL 退款失败
        /// REFUNDING 退款中
        /// </summary>
        public enum OrderStatus
        {
            /// <summary>
            /// 交易成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 交易失败
            /// </summary>
            FAIL,
            /// <summary>
            /// 支付中
            /// </summary>
            PAYING,
            /// <summary>
            /// 已关闭
            /// </summary>
            CLOSED,
            /// <summary>
            /// 已撤销
            /// </summary>
            CANCELED,

            /// <summary>
            /// 退款成功
            /// </summary>
            REFUNDSUC,
            /// <summary>
            /// 退款失败
            /// </summary>
            REFUNDFAIL,
            /// <summary>
            /// 退款中
            /// </summary>
            REFUNDING,
        }

        public static OrderStatus ConvertOrderStatus(string tranSts)
        {
            Enum.TryParse(tranSts, out OrderStatus orderStatus);
            return orderStatus;
        }

        /// <summary>
        /// 支付渠道，枚举值
        /// 取值范围：
        /// WECHAT 微信
        /// ALIPAY 支付宝
        /// UNIONPAY 银联
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetPayType(string wayCode)
        {
            string payType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_LITE:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                    payType = PayType.A02.ToString(); break;
                case CS.PAY_WAY_CODE.ALI_APP:
                    payType = PayType.A03.ToString(); break;
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                    payType = PayType.A01.ToString(); break;
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = PayType.W01.ToString(); break;
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_JSAPI:
                    payType = PayType.W02.ToString(); break;
                case CS.PAY_WAY_CODE.WX_APP:
                    payType = PayType.W03.ToString(); break;
                case CS.PAY_WAY_CODE.WX_LITE:
                    payType = PayType.W06.ToString(); break;
                case CS.PAY_WAY_CODE.UP_QR:
                    payType = PayType.U01.ToString(); break;
                case CS.PAY_WAY_CODE.UP_JSAPI:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = PayType.U02.ToString(); break;
                case CS.PAY_WAY_CODE.DCEP_QR:
                    payType = PayType.S01.ToString(); break;
                default:
                    break;
            }
            return payType;
        }

        /// <summary>
        /// 支付方式，枚举值
        /// 取值范围：
        /// 02 微信公众号 / 支付宝生活号 / 银联js支付 / 支付宝小程序
        /// 03 微信小程序
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetPayWay(string wayCode)
        {
            string payWay = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_LITE:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payWay = PayWay.JSAPI;
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                    payWay = PayWay.LITE;
                    break;
                default:
                    break;
            }
            return payWay;
        }
    }
}
