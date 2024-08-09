using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.Channel.JlPay.Enumerator
{
    public class JlPayEnum
    {
        /// <summary>
        /// 交易类型	
        /// </summary>
        public enum PayType
        {
            wxpay,
            alipay,
            unionpay,
            jlpay
        }

        /// <summary>
        /// 支付渠道，枚举值
        /// 取值范围：
        /// wxpay 微信
        /// alipay 支付宝
        /// unionpay 银联
        /// jlpay 嘉联
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public static string GetPayType(string wayCode)
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
                    payType = PayType.alipay.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_APP:
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = PayType.wxpay.ToString();
                    break;
                case CS.PAY_WAY_CODE.UP_BAR:
                case CS.PAY_WAY_CODE.UP_QR:
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = PayType.unionpay.ToString();
                    break;
                case CS.PAY_WAY_CODE.DCEP_BAR:
                case CS.PAY_WAY_CODE.DCEP_QR:
                    payType = PayType.jlpay.ToString();
                    break;
                default:
                    break;
            }
            return payType;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// 待确认
            /// </summary>
            Pending = 1,
            /// <summary>
            /// 成功
            /// </summary>
            Success = 2,
            /// <summary>
            /// 失败
            /// </summary>
            Failure = 3
        }

        public static Status ConvertStatus(string status)
        {
            Enum.TryParse(status, out Status _status);
            return _status;
        }
    }
}
