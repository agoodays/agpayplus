using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        public static long CalPercentageFee(long amount, decimal rate)
        {
            return CalPercentageFee(amount, rate, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 计算百分比类型的各种费用值  （订单金额 * 真实费率  结果四舍五入并保留0位小数 ）
        /// </summary>
        /// <param name="amount">订单金额  （保持与数据库的格式一致 ，单位：分）</param>
        /// <param name="rate">费率   （保持与数据库的格式一致 ，真实费率值，如费率为0.55%，则传入 0.0055）</param>
        /// <param name="mode">模式 参考：MidpointRounding.AwayFromZero(四舍五入)   MidpointRounding.ToNegativeInfinity（向下取整）   MidpointRounding.ToPositiveInfinity（向上取整）</param>
        /// <returns></returns>
        public static long CalPercentageFee(long amount, decimal rate, MidpointRounding mode)
        {
            //费率乘以订单金额   结果四舍五入并保留0位小数
            return (long)decimal.Round(amount * rate, 0, mode);
        }
    }
}
