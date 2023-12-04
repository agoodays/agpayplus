using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户通知记录表
    /// </summary>
    [Comment("商户通知记录表")]
    [Table("t_mch_notify_record")]
    public class MchNotifyRecord
    {
        /// <summary>
        /// 商户通知记录ID
        /// </summary>
        [Comment("商户通知记录ID")]
        [Key, Required, Column("notify_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long NotifyId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [Comment("订单ID")]
        [Required, Column("order_id", TypeName = "varchar(64)")]
        public string OrderId { get; set; }

        /// <summary>
        /// 订单类型:1-支付,2-退款
        /// </summary>
        [Comment("订单类型:1-支付,2-退款")]
        [Required, Column("order_type", TypeName = "tinyint(6)")]
        public byte OrderType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [Comment("商户订单号")]
        [Required, Column("mch_order_no", TypeName = "varchar(64)")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        [Comment("代理商号")]
        [Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID")]
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        [Comment("通知地址")]
        [Required, Column("notify_url", TypeName = "varchar(128)")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 通知请求方法
        /// </summary>
        [Comment("通知请求方法")]
        [Required, Column("req_method", TypeName = "varchar(10)")]
        public string ReqMethod { get; set; }

        /// <summary>
        /// 通知请求媒体类型
        /// </summary>
        [Comment("通知请求媒体类型")]
        [Required, Column("req_media_type", TypeName = "varchar(64)")]
        public string ReqMediaType { get; set; }

        /// <summary>
        /// 通知请求正文
        /// </summary>
        [Comment("通知请求正文")]
        [Column("req_body", TypeName = "text")]
        public string ReqBody { get; set; }

        /// <summary>
        /// 通知响应结果
        /// </summary>
        [Comment("通知响应结果")]
        [Column("res_result", TypeName = "text")]
        public string ResResult { get; set; }

        /// <summary>
        /// 通知次数
        /// </summary>
        [Comment("通知次数")]
        [Required, Column("notify_count")]
        public int NotifyCount { get; set; }

        /// <summary>
        /// 最大通知次数, 默认6次
        /// </summary>
        [Comment("最大通知次数, 默认6次")]
        [Required, Column("notify_count_limit")]
        public int NotifyCountLimit { get; set; }

        /// <summary>
        /// 通知状态,1-通知中,2-通知成功,3-通知失败
        /// </summary>
        [Comment("通知状态,1-通知中,2-通知成功,3-通知失败")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 最后一次通知时间
        /// </summary>
        [Comment("最后一次通知时间")]
        [Column("last_notify_time")]
        public DateTime? LastNotifyTime { get; set; }

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
