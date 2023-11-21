namespace AGooday.AgPay.Common.Models
{
    public class PageQuery
    {
        /// <summary>
        /// 分页页码
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 分页条数（-1时查全部数据）
        /// </summary>
        public int PageSize { get; set; } = 20;

        public string SortField { get; set; }

        public string SortOrder { get; set; }
    }
}
