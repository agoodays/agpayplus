using System.Globalization;

namespace AGooday.AgPay.Common.Utils
{
    public static class DateUtil
    {
        /// <summary>
        /// 快捷选择选项
        /// 
        /// <code>
        /// 时间段类：
        /// - 'today': 当天
        /// - 'yesterday': 昨天
        /// - 'nearN': 最近N天（N为任意正整数）
        ///   例如：'near1', 'near7', 'near15', 'near30', 'near90' 等
        /// - 'lastNDays': 过去N天（从过去第N天到现在）
        ///   例如：'lastN0Days'(今天), 'lastN7Days', 'lastN30Days' 等
        /// - 'nextNDays': 接下来N天（从今天开始的未来N天）
        ///   例如：'nextN1Days', 'nextN7Days', 'nextN30Days' 等
        /// 
        /// 本期间类：
        /// - 'thisWeek': 本周
        /// - 'thisMonth': 本月
        /// - 'thisQuarter': 本季度
        /// - 'thisYear': 本年
        /// 
        /// 上期间类：
        /// - 'lastWeek': 上周
        /// - 'lastMonth': 上月
        /// - 'lastQuarter': 上季度
        /// - 'lastYear': 上年
        /// - 'lastNMonths': 最后N个月
        /// 例如：'lastN1Months', 'lastN3Months', 'lastN6Months', 'lastN12Months'
        /// - 'lastNQuarters': 最后N个季度
        /// 例如：'lastN1Quarters', 'lastN2Quarters', 'lastN4Quarters'
        /// - 'lastNYears': 最后N年
        /// 例如：'lastN1Years', 'lastN3Years', 'lastN5Years'
        /// 
        /// 下期间类：
        /// - 'nextWeek': 下周
        /// - 'nextMonth': 下月
        /// - 'nextQuarter': 下季度
        /// - 'nextYear': 下年
        /// - 'nextNMonths': 接下来N个月
        /// 例如：'nextN1Months', 'nextN3Months', 'nextN6Months'
        /// - 'nextNQuarters': 接下来N个季度
        /// 例如：'nextN1Quarters', 'nextN2Quarters', 'nextN4Quarters'
        /// - 'nextNYears': 接下来N年
        /// 例如：'nextN1Years', 'nextN3Years', 'nextN5Years'
        /// 
        /// 至今类（财务/数据分析常用）：
        /// - 'ytd': Year-To-Date（今年至今）
        /// - 'mtd': Month-To-Date（本月至今）
        /// - 'qtd': Quarter-To-Date（本季度至今）
        /// 
        /// 上期完整类：
        /// - 'lfy': Last Full Year（去年全年）
        /// - 'lfm': Last Full Month（上月全月）
        /// - 'lfq': Last Full Quarter（上季度全季）
        /// 
        /// 对比类（财务分析常用）：
        /// - 'pyToDate': Prior Year To Date（去年同期至今）
        /// - 'pySameQuarter': Prior Year Same Quarter（去年同期季度）
        /// - 'pyFull': Prior Year Full（去年全年）
        /// 
        /// 其他：
        /// - 'custom': 自定义时间范围
        /// - '': 全部时间（无日期限制）
        /// </code>
        /// </summary>
        public static (DateTime? Start, DateTime? End) GetQueryDateRange(string queryDateRange)
        {
            queryDateRange ??= string.Empty;
            var today = DateTime.Today;

            // 辅助：计算日/季/周的开始与结束
            DateTime StartOfDay(DateTime d) => d.Date;
            DateTime EndOfDay(DateTime d) => d.Date.AddDays(1).AddSeconds(-1);

            DateTime QuarterStart(DateTime d)
            {
                int q = (d.Month - 1) / 3;
                int startMonth = q * 3 + 1;
                return new DateTime(d.Year, startMonth, 1);
            }
            DateTime QuarterEnd(DateTime d)
            {
                var s = QuarterStart(d);
                return s.AddMonths(3).AddSeconds(-1);
            }
            DateTime WeekStartMonday(DateTime d)
            {
                // 以周一为周起始
                int offset = ((int)d.DayOfWeek + 6) % 7;
                return d.Date.AddDays(-offset);
            }

            var q = queryDateRange.Trim();
            if (string.IsNullOrEmpty(q)) return (null, null);

            // ==== 固定短语 ====
            if (q.Equals("today", StringComparison.OrdinalIgnoreCase))
                return (StartOfDay(today), EndOfDay(today));

            if (q.Equals("yesterday", StringComparison.OrdinalIgnoreCase))
            {
                var d = today.AddDays(-1);
                return (StartOfDay(d), EndOfDay(d));
            }

            // 周/上周/下周
            if (q.Equals("thisWeek", StringComparison.OrdinalIgnoreCase))
            {
                var s = WeekStartMonday(today);
                return (StartOfDay(s), EndOfDay(s.AddDays(6)));
            }
            if (q.Equals("lastWeek", StringComparison.OrdinalIgnoreCase))
            {
                var s = WeekStartMonday(today).AddDays(-7);
                return (StartOfDay(s), EndOfDay(s.AddDays(6)));
            }
            if (q.Equals("nextWeek", StringComparison.OrdinalIgnoreCase))
            {
                var s = WeekStartMonday(today).AddDays(7);
                return (StartOfDay(s), EndOfDay(s.AddDays(6)));
            }

            // 月/上月/下月
            if (q.Equals("thisMonth", StringComparison.OrdinalIgnoreCase))
            {
                var s = new DateTime(today.Year, today.Month, 1);
                var e = s.AddMonths(1).AddSeconds(-1);
                return (StartOfDay(s), e);
            }
            if (q.Equals("lastMonth", StringComparison.OrdinalIgnoreCase) || q.Equals("prevMonth", StringComparison.OrdinalIgnoreCase))
            {
                var firstOfThisMonth = new DateTime(today.Year, today.Month, 1);
                var s = firstOfThisMonth.AddMonths(-1);
                var e = firstOfThisMonth.AddSeconds(-1);
                return (StartOfDay(s), e);
            }
            if (q.Equals("nextMonth", StringComparison.OrdinalIgnoreCase))
            {
                var s = new DateTime(today.Year, today.Month, 1).AddMonths(1);
                var e = s.AddMonths(1).AddSeconds(-1);
                return (StartOfDay(s), e);
            }

            // 季度/上季度/下季度
            if (q.Equals("thisQuarter", StringComparison.OrdinalIgnoreCase))
            {
                var s = QuarterStart(today);
                return (StartOfDay(s), QuarterEnd(today));
            }
            if (q.Equals("lastQuarter", StringComparison.OrdinalIgnoreCase))
            {
                var refDate = today.AddMonths(-3);
                var s = QuarterStart(refDate);
                return (StartOfDay(s), QuarterEnd(refDate));
            }
            if (q.Equals("nextQuarter", StringComparison.OrdinalIgnoreCase))
            {
                var refDate = today.AddMonths(3);
                var s = QuarterStart(refDate);
                return (StartOfDay(s), QuarterEnd(refDate));
            }

            // 年/上年/下年
            if (q.Equals("thisYear", StringComparison.OrdinalIgnoreCase))
            {
                var s = new DateTime(today.Year, 1, 1);
                return (StartOfDay(s), EndOfDay(today));
            }
            if (q.Equals("lastYear", StringComparison.OrdinalIgnoreCase))
            {
                var y = today.Year - 1;
                var s = new DateTime(y, 1, 1);
                var e = new DateTime(y, 12, 31, 23, 59, 59);
                return (StartOfDay(s), e);
            }
            if (q.Equals("nextYear", StringComparison.OrdinalIgnoreCase))
            {
                var s = new DateTime(today.Year + 1, 1, 1);
                var e = new DateTime(today.Year + 1, 12, 31, 23, 59, 59);
                return (StartOfDay(s), e);
            }

            // ==== 至今类（本年至今、本月至今、本季度至今） ====
            if (q.Equals("ytd", StringComparison.OrdinalIgnoreCase))
            {
                var s = new DateTime(today.Year, 1, 1);
                return (StartOfDay(s), EndOfDay(today));
            }
            if (q.Equals("mtd", StringComparison.OrdinalIgnoreCase) || q.Equals("currMonth", StringComparison.OrdinalIgnoreCase))
            {
                var s = new DateTime(today.Year, today.Month, 1);
                return (StartOfDay(s), EndOfDay(today));
            }
            if (q.Equals("qtd", StringComparison.OrdinalIgnoreCase))
            {
                var s = QuarterStart(today);
                return (StartOfDay(s), EndOfDay(today));
            }

            // ==== 上期完整类 ====
            if (q.Equals("lfy", StringComparison.OrdinalIgnoreCase) || q.Equals("pyFull", StringComparison.OrdinalIgnoreCase))
            {
                var y = today.Year - 1;
                var s = new DateTime(y, 1, 1);
                var e = new DateTime(y, 12, 31, 23, 59, 59);
                return (StartOfDay(s), e);
            }
            if (q.Equals("lfm", StringComparison.OrdinalIgnoreCase))
            {
                var firstOfThisMonth = new DateTime(today.Year, today.Month, 1);
                var s = firstOfThisMonth.AddMonths(-1);
                var e = firstOfThisMonth.AddSeconds(-1);
                return (StartOfDay(s), e);
            }
            if (q.Equals("lfq", StringComparison.OrdinalIgnoreCase))
            {
                var refDate = today.AddMonths(-3);
                var s = QuarterStart(refDate);
                var e = QuarterEnd(refDate);
                return (StartOfDay(s), e);
            }

            // ==== 对比类（去年同期类） ====
            if (q.Equals("pyToDate", StringComparison.OrdinalIgnoreCase))
            {
                var y = today.Year - 1;
                var s = new DateTime(y, 1, 1);
                int day = Math.Min(DateTime.DaysInMonth(y, today.Month), today.Day);
                var e = new DateTime(y, today.Month, day, 23, 59, 59);
                return (StartOfDay(s), e);
            }
            if (q.Equals("pySameQuarter", StringComparison.OrdinalIgnoreCase))
            {
                var refDate = today.AddYears(-1);
                var s = QuarterStart(refDate);
                var e = QuarterEnd(refDate);
                return (StartOfDay(s), e);
            }

            // ==== 动态模式解析（nearN, lastN* , nextN* 等） ====
            try
            {
                // nearN，例如 near7 => 最近 7 天（包含今天）
                if (q.StartsWith("near", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(4);
                    if (int.TryParse(numStr, out int days) && days > 0)
                    {
                        var s = today.AddDays(-(days - 1));
                        return (StartOfDay(s), EndOfDay(today));
                    }
                }

                // lastN{Days|Months|Quarters|Years}
                if (q.StartsWith("lastN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Days", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Days".Length);
                    if (int.TryParse(numStr, out int days) && days >= 0)
                    {
                        var s = today.AddDays(-days);
                        return (StartOfDay(s), EndOfDay(today));
                    }
                }
                if (q.StartsWith("nextN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Days", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Days".Length);
                    if (int.TryParse(numStr, out int days) && days > 0)
                    {
                        var e = today.AddDays(days - 1);
                        return (StartOfDay(today), EndOfDay(e));
                    }
                }

                // lastNMonths
                if (q.StartsWith("lastN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Months", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Months".Length);
                    if (int.TryParse(numStr, out int months) && months > 0)
                    {
                        var s = new DateTime(today.Year, today.Month, 1).AddMonths(-months);
                        return (StartOfDay(s), EndOfDay(today));
                    }
                }
                // nextNMonths
                if (q.StartsWith("nextN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Months", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Months".Length);
                    if (int.TryParse(numStr, out int months) && months > 0)
                    {
                        var s = new DateTime(today.Year, today.Month, 1).AddMonths(1);
                        var e = s.AddMonths(months - 1).AddMonths(1).AddSeconds(-1);
                        return (StartOfDay(s), e);
                    }
                }

                // lastNQuarters / nextNQuarters
                if (q.StartsWith("lastN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Quarters", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Quarters".Length);
                    if (int.TryParse(numStr, out int quarters) && quarters > 0)
                    {
                        var s = QuarterStart(today).AddMonths(-3 * quarters);
                        return (StartOfDay(s), EndOfDay(today));
                    }
                }
                if (q.StartsWith("nextN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Quarters", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Quarters".Length);
                    if (int.TryParse(numStr, out int quarters) && quarters > 0)
                    {
                        var s = QuarterStart(today).AddMonths(3);
                        var e = s.AddMonths(3 * quarters - 1).AddMonths(1).AddSeconds(-1);
                        return (StartOfDay(s), e);
                    }
                }

                // lastNYears / nextNYears
                if (q.StartsWith("lastN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Years", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Years".Length);
                    if (int.TryParse(numStr, out int years) && years > 0)
                    {
                        var s = new DateTime(today.Year - years, 1, 1);
                        return (StartOfDay(s), EndOfDay(today));
                    }
                }
                if (q.StartsWith("nextN", StringComparison.OrdinalIgnoreCase) && q.EndsWith("Years", StringComparison.OrdinalIgnoreCase))
                {
                    var numStr = q.Substring(5, q.Length - 5 - "Years".Length);
                    if (int.TryParse(numStr, out int years) && years > 0)
                    {
                        var s = new DateTime(today.Year + 1, 1, 1);
                        var e = s.AddYears(years).AddSeconds(-1);
                        return (StartOfDay(s), e);
                    }
                }
            }
            catch
            {
                // 解析异常：忽略并继续尝试 custom 或返回 null
            }

            // ==== 自定义格式支持 custom_yyyy-MM-dd[_HH:mm:ss] 或 customDateTime_... ====
            if (q.StartsWith("custom_", StringComparison.OrdinalIgnoreCase) || q.StartsWith("customDateTime_", StringComparison.OrdinalIgnoreCase))
            {
                var parts = q.Split(new[] { '_' }, 3);
                if (parts.Length >= 3 && DateTime.TryParse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var s) && DateTime.TryParse(parts[2], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var e))
                {
                    return (StartOfDay(s), EndOfDay(e));
                }
            }

            // 未匹配：返回空，调用方决定是否覆盖已有日期条件
            return (null, null);
        }
    }
}
