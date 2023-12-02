using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 退款订单表
    /// </summary>
    [Comment("退款订单表")]
    [Table("t_refund_order")]
    public class RefundOrder
    {
        /// <summary>
        /// 退款订单号（支付系统生成订单号）
        /// </summary>
        [Comment("退款订单号（支付系统生成订单号）")]
        [Key, Required, Column("refund_order_id", TypeName = "varchar(30)")]
        public string RefundOrderId { get; set; }

        /// <summary>
        /// 支付订单号（与t_pay_order对应）
        /// </summary>
        [Comment("支付订单号（与t_pay_order对应）")]
        [Required, Column("pay_order_id", TypeName = "varchar(30)")]
        public string PayOrderId { get; set; }

        /// <summary>
        /// 渠道支付单号（与t_pay_order channel_order_no对应）
        /// </summary>
        [Comment("渠道支付单号（与t_pay_order channel_order_no对应）")]
        [Column("channel_pay_order_no", TypeName = "varchar(64)")]
        public string ChannelPayOrderNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [Comment("商户名称")]
        [Required, Column("mch_name", TypeName = "varchar(64)")]
        public string MchName { get; set; }

        /// <summary>
        /// 商户简称
        /// </summary>
        [Comment("商户简称")]
        [Column("mch_short_name", TypeName = "varchar(32)")]
        public string MchShortName { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        [Comment("代理商号")]
        [Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [Comment("代理商名称")]
        [Column("agent_name", TypeName = "varchar(64)")]
        public string AgentName { get; set; }

        /// <summary>
        /// 代理商简称
        /// </summary>
        [Comment("代理商简称")]
        [Column("agent_short_name", TypeName = "varchar(32)")]
        public string AgentShortName { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 服务商名称
        /// </summary>
        [Comment("服务商名称")]
        [Column("isv_name", TypeName = "varchar(64)")]
        public string IsvName { get; set; }

        /// <summary>
        /// 服务商简称
        /// </summary>
        [Comment("服务商简称")]
        [Column("isv_short_name", TypeName = "varchar(32)")]
        public string IsvShortName { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID")]
        [Required, Column("app_id", TypeName = "varchar(64)")]
        public string AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [Comment("应用名称")]
        [Column("app_name", TypeName = "varchar(64)")]
        public string AppName { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [Comment("门店ID")]
        [Column("store_id", TypeName = "varchar(64)")]
        public string StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [Comment("门店名称")]
        [Column("store_name", TypeName = "varchar(64)")]
        public string StoreName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [Comment("类型: 1-普通商户, 2-特约商户(服务商模式)")]
        [Required, Column("mch_type", TypeName = "tinyint(6)")]
        public byte MchType { get; set; }

        /// <summary>
        /// 商户退款单号（商户系统的订单号）
        /// </summary>
        [Comment("商户退款单号（商户系统的订单号）")]
        [Required, Column("mch_refund_no", TypeName = "varchar(64)")]
        public string MchRefundNo { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        [Comment("支付方式代码")]
        [Required, Column("way_code", TypeName = "varchar(20)")]
        public string WayCode { get; set; }

        /// <summary>
        /// 支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他
        /// </summary>
        [Comment("支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他")]
        [Required, Column("way_type", TypeName = "varchar(20)")]
        public string WayType { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Comment("支付接口代码")]
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 支付金额,单位分
        /// </summary>
        [Comment("支付金额,单位分")]
        [Required, Column("pay_amount", TypeName = "bigint")]
        public long PayAmount { get; set; }

        /// <summary>
        /// 退款金额,单位分
        /// </summary>
        [Comment("退款金额,单位分")]
        [Required, Column("refund_amount", TypeName = "bigint")]
        public long RefundAmount { get; set; }

        /// <summary>
        /// 手续费退还金额,单位分
        /// </summary>
        [Comment("手续费退还金额,单位分")]
        [Required, Column("refund_fee_amount", TypeName = "bigint")]
        public long RefundFeeAmount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        [Comment("三位货币代码,人民币:cny")]
        [Required, Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        /// <summary>
        /// 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败,4-退款任务关闭
        /// </summary>
        [Comment("退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败,4-退款任务关闭")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [Comment("客户端IP")]
        [Column("client_ip", TypeName = "varchar(32)")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [Comment("退款原因")]
        [Required, Column("refund_reason", TypeName = "varchar(256)")]
        public string RefundReason { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        [Comment("渠道订单号")]
        [Column("channel_order_no", TypeName = "varchar(32)")]
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道错误码
        /// </summary>
        [Comment("渠道错误码")]
        [Column("err_code", TypeName = "varchar(128)")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道错误描述
        /// </summary>
        [Comment("渠道错误描述")]
        [Column("err_msg", TypeName = "varchar(256)")]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 特定渠道发起时额外参数
        /// </summary>
        [Comment("特定渠道发起时额外参数")]
        [Column("channel_extra", TypeName = "varchar(512)")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        [Comment("通知地址")]
        [Column("notify_url", TypeName = "varchar(128)")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [Comment("扩展参数")]
        [Column("ext_param", TypeName = "varchar(64)")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 订单退款成功时间
        /// </summary>
        [Comment("订单退款成功时间")]
        [Column("success_time", TypeName = "datetime")]
        public DateTime? SuccessTime { get; set; }

        /// <summary>
        /// 退款失效时间（失效后系统更改为退款任务关闭状态）
        /// </summary>
        [Comment("退款失效时间（失效后系统更改为退款任务关闭状态）")]
        [Column("expired_time", TypeName = "datetime")]
        public DateTime? ExpiredTime { get; set; }

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
