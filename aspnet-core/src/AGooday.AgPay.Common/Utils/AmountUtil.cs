namespace AGooday.AgPay.Common.Utils
{
    public class AmountUtil
    {
        /// <summary>
        /// 计算百分比类型的各种费用值  （订单金额 * 真实费率  结果四舍五入并保留0位小数 ）
        /// </summary>
        /// <param name="amount">订单金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="rate">费率   （保持与数据库的格式一致 ，真实费率值，如费率为0.55%，则传入 0.0055）</param>
        /// <returns></returns>
        public static long CalPercentageFee(long amount, decimal rate) => CalPercentageFee(amount, rate, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 计算百分比类型的各种费用值  （订单金额 * 真实费率  结果四舍五入/向下取整/向上取整并保留0位小数 ）
        /// </summary>
        /// <param name="amount">订单金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="rate">费率   （保持与数据库的格式一致 ，真实费率值，如费率为0.55%，则传入 0.0055）</param>
        /// <param name="mode">模式 参考：MidpointRounding.AwayFromZero(四舍五入)   MidpointRounding.ToNegativeInfinity（向下取整）   MidpointRounding.ToPositiveInfinity（向上取整）</param>
        /// <returns></returns>
        public static long CalPercentageFee(long amount, decimal rate, MidpointRounding mode)
        {
            // 费率乘以订单金额   结果四舍五入并保留0位小数
            return (long)decimal.Round(amount * rate, 0, mode);
        }

        /// <summary>
        /// 计算百分比类型的各种费用值  （订单金额 * 真实费率  结果四舍五入并保留0位小数 ）
        /// </summary>
        /// <param name="refundAmount">退款金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="payAmount">支付金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="mchOrderFeeAmount">收单手续费  （保持与数据库的格式一致 ，单位：分）</param>
        /// <returns></returns>
        public static long CalPercentageFee(long refundAmount, long payAmount, long mchOrderFeeAmount) => CalPercentageFee(refundAmount, payAmount, mchOrderFeeAmount, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 计算百分比类型的各种费用值  （订单金额 * 真实费率  结果四舍五入/向下取整/向上取整并保留0位小数 ）
        /// </summary>
        /// <param name="refundAmount">退款金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="payAmount">支付金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="mchOrderFeeAmount">收单手续费  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="mode">模式 参考：MidpointRounding.AwayFromZero(四舍五入)   MidpointRounding.ToNegativeInfinity（向下取整）   MidpointRounding.ToPositiveInfinity（向上取整）</param>
        /// <returns></returns>
        public static long CalPercentageFee(long refundAmount, long payAmount, long mchOrderFeeAmount, MidpointRounding mode)
        {
            // 费率乘以订单金额   结果四舍五入并保留0位小数
            return (long)decimal.Round(refundAmount / (decimal)payAmount * mchOrderFeeAmount, 0, mode);
        }

        /// <summary>
        /// 将Long "分"转换成"元"（长格式），如：100分被转换为1.00元。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ConvertCent2Dollar(string amount) => ConvertCent2Dollar(Convert.ToInt64(amount));

        /// <summary>
        /// 将Long "分"转换成"元"（长格式），如：100分被转换为1.00元。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ConvertCent2Dollar(long amount) => (Convert.ToDouble(amount) / 100).ToString("0.00");

        /// <summary>
        /// 将"元"转换成"分"，如：1.00分被转换为100元。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static long ConvertDollar2Cent(string amount) => ConvertDollar2Cent(Convert.ToDecimal(amount));

        /// <summary>
        /// 将"元"转换成"分"，如：1.00分被转换为100元。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static long ConvertDollar2Cent(decimal amount) => Convert.ToInt64(amount * 100);
    }
}
