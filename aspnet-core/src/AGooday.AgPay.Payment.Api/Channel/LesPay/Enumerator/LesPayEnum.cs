using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay.Enumerator
{
    public class LesPayEnum
    {
        /// <summary>
        /// 支付类型
        /// WXZF	微信
        /// ZFBZF 支付宝
        /// UPSMZF  银联二维码
        /// DCPAY 数字货币
        /// </summary>
        public enum PayWay
        {
            /// <summary>
            /// 微信
            /// </summary>
            WXZF,
            /// <summary>
            /// 支付宝
            /// </summary>
            ZFBZF,
            /// <summary>
            /// 银联二维码
            /// </summary>
            UPSMZF,
            /// <summary>
            /// 数字货币
            /// </summary>
            DCPAY
        }

        /// <summary>
        /// 支付类型
        /// 0-支付宝Native扫码支付、银联Native扫码支付；
        /// 1-微信JSAPI、支付宝JSAPI支付、银联JSAPI支付；
        /// 2-微信、支付宝简易支付<跳转乐刷收银台支付>；
        /// （jspay_flag=2时必传jump_url，否则会报错）
        /// 3-微信小程序支付、支付宝小程序支付
        /// 注：
        /// 1）微信拉码支付已下线；
        /// 2）如需接入银联JS支付，请联系乐刷运营沟通域名报备。
        /// 3）数字货币支付当前仅支持jspay_flag=0
        /// </summary>
        public enum JspayFlag
        {
            /// <summary>
            /// 支付宝Native扫码支付、银联Native扫码支付；
            /// </summary>
            Native = 0,
            /// <summary>
            /// 微信JSAPI、支付宝JSAPI支付、银联JSAPI支付；
            /// </summary>
            JSAPI = 1,
            /// <summary>
            /// 微信、支付宝简易支付<跳转乐刷收银台支付>；
            /// （jspay_flag = 2时必传jump_url，否则会报错）
            /// </summary>
            Cashier = 2,
            /// <summary>
            /// 微信小程序支付、支付宝小程序支付
            /// </summary>
            Lite = 3
        }

        /// <summary>
        /// 订单状态
        /// 0	支付中
        /// 2	支付成功
        /// 6	订单关闭
        /// 8	支付失败
        /// 10	退款中
        /// 11	退款成功
        /// 12	退款失败
        /// </summary>
        public enum OrderStatus
        {
            /// <summary>
            /// 支付中
            /// </summary>
            Paying = 0,
            /// <summary>
            /// 支付成功
            /// </summary>
            PaySuccess = 2,
            /// <summary>
            /// 订单关闭
            /// </summary>
            PayClosed = 6,
            /// <summary>
            /// 支付失败
            /// </summary>
            PayFail = 8,

            /// <summary>
            /// 退款中
            /// </summary>
            Refunding = 10,
            /// <summary>
            /// 退款成功
            /// </summary>
            RefundSuccess = 11,
            /// <summary>
            /// 退款失败
            /// </summary>
            RefundFail = 12,
        }

        public static OrderStatus ConvertOrderStatus(string status)
        {
            Enum.TryParse(status, out OrderStatus orderStatus);
            return orderStatus;
        }

        public static string GetPayWay(string wayCode)
        {
            string payType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_BAR:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_APP:
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payType = PayWay.ZFBZF.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = PayWay.WXZF.ToString();
                    break;
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = PayWay.UPSMZF.ToString();
                    break;
                default:
                    break;
            }
            return payType;
        }


        /// <summary>
        /// 支付类型
        /// 0-支付宝Native扫码支付、银联Native扫码支付；
        /// 1-微信JSAPI、支付宝JSAPI支付、银联JSAPI支付；
        /// 2-微信、支付宝简易支付<跳转乐刷收银台支付>；
        /// （jspay_flag=2时必传jump_url，否则会报错）
        /// 3-微信小程序支付、支付宝小程序支付
        /// 注：
        /// 1）微信拉码支付已下线；
        /// 2）如需接入银联JS支付，请联系乐刷运营沟通域名报备。
        /// 3）数字货币支付当前仅支持jspay_flag=0
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetJspayFlag(string wayCode)
        {
            string payWay = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_WAP:
                    payWay = JspayFlag.Native.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payWay = JspayFlag.JSAPI.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payWay = JspayFlag.Lite.ToString();
                    break;
                default:
                    break;
            }
            return payWay;
        }
    }
}
