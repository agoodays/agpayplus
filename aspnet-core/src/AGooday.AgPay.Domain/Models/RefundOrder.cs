using AGooday.AgPay.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 退款订单表
    /// </summary>
    [Table("t_refund_order")]
    public class RefundOrder
    {
        /// <summary>
        /// 退款订单号（支付系统生成订单号）
        /// </summary>
        [Key, Required, Column("refund_order_id", TypeName = "varchar(30)")]
        public string RefundOrderId { get; set; }

        /// <summary>
        /// 支付订单号（与t_pay_order对应）
        /// </summary>
        [Required, Column("pay_order_id", TypeName = "varchar(30)")]
        public string PayOrderId { get; set; }

        /// <summary>
        /// 渠道支付单号（与t_pay_order channel_order_no对应）
        /// </summary>
        [Column("channel_pay_order_no", TypeName = "varchar(64)")]
        public string ChannelPayOrderNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [Required, Column("mch_name", TypeName = "varchar(30)")]
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [Required, Column("mch_type", TypeName = "tinyint")]
        public byte MchType { get; set; }

        /// <summary>
        /// 商户退款单号（商户系统的订单号）
        /// </summary>
        [Required, Column("mch_order_no", TypeName = "varchar(64)")]
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        [Required, Column("way_code", TypeName = "varchar(20)")]
        public string WayCode { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 支付金额,单位分
        /// </summary>
        [Required, Column("pay_amount", TypeName = "bigint")]
        public long PayAmount { get; set; }

        /// <summary>
        /// 退款金额,单位分
        /// </summary>
        [Required, Column("refund_amount", TypeName = "bigint")]
        public long RefundAmount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        [Required, Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        /// <summary>
        /// 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败,4-退款任务关闭
        /// </summary>
        [Required, Column("state", TypeName = "tinyint")]
        public byte State { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [Column("client_ip", TypeName = "varchar(32)")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [Required, Column("refund_reason", TypeName = "varchar(256)")]
        public string RefundReason { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        [Required, Column("channel_order_no", TypeName = "varchar(32)")]
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道错误码
        /// </summary>
        [Column("err_code", TypeName = "varchar(128)")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道错误描述
        /// </summary>
        [Column("err_msg", TypeName = "varchar(256)")]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 特定渠道发起时额外参数
        /// </summary>
        [Column("channel_extra", TypeName = "varchar(512)")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        [Column("notify_url", TypeName = "varchar(128)")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [Column("ext_param", TypeName = "varchar(64)")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 订单退款成功时间
        /// </summary>
        [Column("success_time", TypeName = "datetime")]
        public DateTime SuccessTime { get; set; }

        /// <summary>
        /// 退款失效时间（失效后系统更改为退款任务关闭状态）
        /// </summary>
        [Column("expired_time", TypeName = "datetime")]
        public DateTime ExpiredTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
