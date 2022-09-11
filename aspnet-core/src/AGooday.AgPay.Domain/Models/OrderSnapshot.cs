using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 订单接口数据快照
    /// </summary>
    [Table("t_order_snapshot")]
    public class OrderSnapshot
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [Required, Column("order_id", TypeName = "varchar(64)")]
        public string OrderId { get; set; }

        /// <summary>
        /// 订单类型: 1-支付, 2-退款
        /// </summary>
        [Required, Column("order_type", TypeName = "tinyint(6)")]
        public byte OrderType { get; set; }

        /// <summary>
        /// 下游请求数据
        /// </summary>
        [Column("mch_req_data", TypeName = "text")]
        public string MchReqData { get; set; }

        /// <summary>
        /// 下游请求时间
        /// </summary>
        [Column("mch_req_time", TypeName = "datetime")]
        public DateTime MchReqTime { get; set; }

        /// <summary>
        /// 向下游响应数据
        /// </summary>
        [Column("mch_resp_data", TypeName = "text")]
        public string MchRespData { get; set; }

        /// <summary>
        /// 向下游响应时间
        /// </summary>
        [Column("mch_resp_time", TypeName = "datetime")]
        public DateTime MchRespTime { get; set; }

        /// <summary>
        /// 向上游请求数据
        /// </summary>
        [Column("channel_req_data", TypeName = "text")]
        public string ChannelReqData { get; set; }

        /// <summary>
        /// 向上游请求时间
        /// </summary>
        [Column("channel_req_time", TypeName = "datetime")]
        public DateTime ChannelReqTime { get; set; }

        /// <summary>
        /// 上游响应数据
        /// </summary>
        [Column("channel_resp_data", TypeName = "text")]
        public string ChannelRespData { get; set; }

        /// <summary>
        /// 上游响应时间
        /// </summary>
        [Column("channel_resp_time", TypeName = "datetime")]
        public DateTime ChannelRespTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required, Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Required, Column("updated_at", TypeName = "timestamp")]
        public DateTime UpdatedAt { get; set; }
    }
}
