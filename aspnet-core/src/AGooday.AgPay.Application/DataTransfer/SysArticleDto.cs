using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 公告信息表
    /// </summary>
    public class SysArticleDto
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        public long ArticleId { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 公告副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 公告类型
        /// </summary>
        public byte ArticleType { get; set; }

        /// <summary>
        /// 公告范围 ["MCH", "AGENT"]
        /// </summary>
        public JArray ArticleRange { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
