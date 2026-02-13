using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Common.Models
{
    public class DatePageQuery : PageQuery
    {
        public virtual void BindDateRange()
        {
            var (start, end) = DateUtil.GetQueryDateRange(QueryDateRange);
            if (start.HasValue) CreatedStart = start.Value;
            if (end.HasValue) CreatedEnd = end.Value;
        }

        public string QueryDateRange { get; set; } = string.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? CreatedStart { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? CreatedEnd { get; set; }
    }
}
