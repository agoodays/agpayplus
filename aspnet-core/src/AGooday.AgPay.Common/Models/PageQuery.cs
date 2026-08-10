namespace AGooday.AgPay.Common.Models
{
    public class PageQuery
    {
        /// <summary>
        /// 升序
        /// </summary>
        public const string ASCEND = "ascend";

        /// <summary>
        /// 降序
        /// </summary>
        public const string DESCEND = "descend";

        /// <summary>
        /// 分页页码
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 分页条数（-1时查全部数据）
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string SortOrder { get; set; }
        
        /// <summary>
        /// 关键词：商户号 / 商户全称 / 商户简称（模糊匹配，优先级低于 MchNo 精确匹配）
        /// </summary>
        public string Keyword { get; set; }
    }
}
