using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.Channel.DgPay.Enumerator
{
    public class DgPayEnum
    {
        /// <summary>
        /// 交易类型	
        /// </summary>
        public enum TransType
        {
            /// <summary>
            /// 微信公众号支付
            /// </summary>
            T_JSAPI,
            /// <summary>
            /// 微信小程序支付
            /// </summary>
            T_MINIAPP,
            /// <summary>
            /// 支付宝JS
            /// </summary>
            A_JSAPI,
            /// <summary>
            /// 支付宝正扫
            /// </summary>
            A_NATIVE,
            /// <summary>
            /// 银联正扫
            /// </summary>
            U_NATIVE,
            /// <summary>
            /// 银联 JS
            /// </summary>
            U_JSAPI,
            /// <summary>
            /// 微信反扫
            /// </summary>
            T_MICROPAY,
            /// <summary>
            /// 支付宝反扫
            /// </summary>
            A_MICROPAY,
            /// <summary>
            /// 银联反扫
            /// </summary>
            U_MICROPAY,
            /// <summary>
            /// 数字人民币正扫
            /// </summary>
            D_NATIVE,
            /// <summary>
            /// 数字人民币反扫
            /// </summary>
            D_MICROPAY,
            /// <summary>
            /// 微信直连H5支付
            /// </summary>
            T_H5,
            /// <summary>
            /// 微信APP支付（只支持直连）
            /// </summary>
            T_APP,
            /// <summary>
            /// 微信正扫（只支持直连）
            /// </summary>
            T_NATIVE
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
        public static string GetTransType(string wayCode)
        {
            string payType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_BAR:
                    payType = TransType.A_MICROPAY.ToString();
                    break;
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                    payType = TransType.A_JSAPI.ToString();
                    break;
                case CS.PAY_WAY_CODE.ALI_APP:
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    payType = TransType.A_NATIVE.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_BAR:
                    payType = TransType.T_MICROPAY.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_APP:
                    payType = TransType.T_APP.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                    payType = TransType.T_JSAPI.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                    payType = TransType.T_MINIAPP.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_H5:
                    payType = TransType.T_H5.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    payType = TransType.T_NATIVE.ToString();
                    break;
                case CS.PAY_WAY_CODE.UP_BAR:
                    payType = TransType.U_MICROPAY.ToString();
                    break;
                case CS.PAY_WAY_CODE.UP_QR:
                    payType = TransType.U_NATIVE.ToString();
                    break;
                case CS.PAY_WAY_CODE.YSF_BAR:
                    payType = TransType.U_MICROPAY.ToString();
                    break;
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    payType = TransType.U_JSAPI.ToString();
                    break;
                case CS.PAY_WAY_CODE.DCEP_BAR:
                    payType = TransType.D_MICROPAY.ToString();
                    break;
                case CS.PAY_WAY_CODE.DCEP_QR:
                    payType = TransType.D_NATIVE.ToString();
                    break;
                default:
                    break;
            }
            return payType;
        }

        /// <summary>
        /// 交易状态
        /// </summary>
        public enum TransStat
        {
            /// <summary>
            /// 初始（初始状态很罕见，请联系汇付技术人员处理）
            /// </summary>
            I,
            /// <summary>
            /// 处理中
            /// </summary>
            P,
            /// <summary>
            /// 成功
            /// </summary>
            S,
            /// <summary>
            /// 失败
            /// </summary>
            F
        }

        public static TransStat ConvertTransStat(string transStat)
        {
            Enum.TryParse(transStat, out TransStat _transStat);
            return _transStat;
        }
    }
}
