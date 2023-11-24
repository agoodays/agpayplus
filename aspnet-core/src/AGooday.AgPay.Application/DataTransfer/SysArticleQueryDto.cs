using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 文章信息表
    /// </summary>
    public class SysArticleQueryDto : DatePageQuery
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public long? ArticleId { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文章类型: 1-公告
        /// </summary>
        public byte? ArticleType { get; set; }

        /// <summary>
        /// 文章范围
        /// </summary>
        public string ArticleRange { get; set; }
    }
}
