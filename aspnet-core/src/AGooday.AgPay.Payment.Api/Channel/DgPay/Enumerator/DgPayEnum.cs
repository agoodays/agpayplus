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
            T_H5
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
