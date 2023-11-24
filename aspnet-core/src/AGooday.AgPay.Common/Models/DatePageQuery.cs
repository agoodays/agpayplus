using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Common.Models
{
    public class DatePageQuery : PageQuery
    {
        public void BindDateRange()
        {
            QueryDateRange = QueryDateRange ?? string.Empty;
            DateUtil.GetQueryDateRange(QueryDateRange, out string createdStart, out string createdEnd);
            CreatedStart = string.IsNullOrWhiteSpace(createdStart) ? CreatedStart : Convert.ToDateTime(createdStart);
            CreatedEnd = string.IsNullOrWhiteSpace(createdEnd) ? CreatedEnd : Convert.ToDateTime(createdEnd);
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
