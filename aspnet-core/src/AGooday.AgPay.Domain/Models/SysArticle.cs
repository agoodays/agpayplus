using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 文章信息表
    /// </summary>
    [Comment("文章信息表")]
    [Table("t_sys_article")]
    public class SysArticle
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Comment("文章ID")]
        [Key, Required, Column("article_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long ArticleId { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [Comment("文章标题")]
        [Required, Column("title", TypeName = "varchar(64)")]
        public string Title { get; set; }

        /// <summary>
        /// 文章副标题
        /// </summary>
        [Comment("文章副标题")]
        [Required, Column("subtitle", TypeName = "varchar(64)")]
        public string Subtitle { get; set; }

        /// <summary>
        /// 文章类型: 1-公告
        /// </summary>
        [Comment("文章类型: 1-公告")]
        [Required, Column("article_type", TypeName = "tinyint(6)")]
        public byte ArticleType { get; set; }

        /// <summary>
        /// 文章范围 ["MCH", "AGENT"]
        /// </summary>
        [Comment("文章范围 [\"MCH\", \"AGENT\"]")]
        [Required, Column("article_range", TypeName = "json")]
        public string ArticleRange { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        [Comment("文章内容")]
        [Column("content", TypeName = "text")]
        public string Content { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        [Comment("发布人")]
        [Required, Column("publisher", TypeName = "varchar(32)")]
        public string Publisher { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [Comment("发布时间")]
        [Column("publish_time", TypeName = "timestamp(6)")]
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}
