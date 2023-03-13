namespace AGooday.AgPay.Common.Models
{
    public class DatePageQuery : PageQuery
    {
        public void BindDateRange()
        {
            string createdStart = null, createdEnd = null;
            QueryDateRange = QueryDateRange ?? string.Empty;
            if (QueryDateRange.Equals("today"))
            {
                createdStart = DateTime.Today.ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            if (QueryDateRange.Equals("yesterday"))
            {
                createdStart = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.ToString("yyyy-MM-dd");
            }
            if (QueryDateRange.Contains("near2now"))
            {
                int day = Convert.ToInt32(QueryDateRange.Split("_")[1]);
                createdStart = DateTime.Today.AddDays(-day).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.ToString("yyyy-MM-dd");
            }
            if (QueryDateRange.Contains("customDateTime"))
            {
                createdStart = Convert.ToDateTime(QueryDateRange.Split("_")[1]).ToString("yyyy-MM-dd HH:mm:dd");
                createdEnd = Convert.ToDateTime(QueryDateRange.Split("_")[2]).AddDays(1).ToString("yyyy-MM-dd HH:mm:dd");
            }
            CreatedStart = string.IsNullOrWhiteSpace(createdStart) ? CreatedStart : Convert.ToDateTime(createdStart);
            CreatedEnd = string.IsNullOrWhiteSpace(createdEnd) ? CreatedStart : Convert.ToDateTime(createdEnd);
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
