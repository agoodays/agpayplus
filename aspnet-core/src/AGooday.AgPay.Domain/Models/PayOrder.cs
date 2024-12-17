using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 支付订单表
    /// </summary>
    [Comment("支付订单表")]
    [Table("t_pay_order")]
    public class PayOrder : AbstractTrackableTimestamps
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        [Comment("支付订单号")]
        [Key, Required, Column("pay_order_id", TypeName = "varchar(30)")]
        public string PayOrderId { get; set; }

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
        [Column("store_id", TypeName = "bigint(20)")]
        public long? StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [Comment("门店名称")]
        [Column("store_name", TypeName = "varchar(64)")]
        public string StoreName { get; set; }

        /// <summary>
        /// 二维码ID
        /// </summary>
        [Comment("二维码ID")]
        [Column("qrc_id", TypeName = "varchar(64)")]
        public string QrcId { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [Comment("类型: 1-普通商户, 2-特约商户(服务商模式)")]
        [Required, Column("mch_type", TypeName = "tinyint(6)")]
        public byte MchType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [Comment("商户订单号")]
        [Required, Column("mch_order_no", TypeName = "varchar(64)")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Comment("支付接口代码")]
        [Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

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
        /// 支付金额,单位分
        /// </summary>
        [Comment("支付金额,单位分")]
        [Required, Column("amount", TypeName = "bigint(20)")]
        public long Amount { get; set; }

        /// <summary>
        /// 商户手续费费率快照
        /// </summary>
        [Comment("商户手续费费率快照")]
        [Required, Column("mch_fee_rate", TypeName = "decimal(20,6)")]
        public decimal MchFeeRate { get; set; }

        /// <summary>
        /// 商户手续费费率快照描述
        /// </summary>
        [Comment("商户手续费费率快照描述")]
        [Column("mch_fee_rate_desc", TypeName = "varchar(128)")]
        public string MchFeeRateDesc { get; set; }

        /// <summary>
        /// 商户手续费(实际手续费),单位分
        /// </summary>
        [Comment("商户手续费(实际手续费),单位分")]
        [Required, Column("mch_fee_amount", TypeName = "bigint(20)")]
        public long MchFeeAmount { get; set; }

        /// <summary>
        /// 收单手续费,单位分
        /// </summary>
        [Comment("收单手续费,单位分")]
        [Required, Column("mch_order_fee_amount", TypeName = "bigint(20)")]
        public long MchOrderFeeAmount { get; set; }

        /// <summary>
        /// 三位货币代码, 人民币: CNY
        /// </summary>
        [Comment("三位货币代码, 人民币: CNY")]
        [Required, Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        /// <summary>
        /// 支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭
        /// </summary>
        [Comment("支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 向下游回调状态, 0-未发送,  1-已发送
        /// </summary>
        [Comment("向下游回调状态, 0-未发送,  1-已发送")]
        [Required, Column("notify_state", TypeName = "tinyint(6)")]
        public byte NotifyState { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [Comment("客户端IP")]
        [Column("client_ip", TypeName = "varchar(32)")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        [Comment("商品标题")]
        [Required, Column("subject", TypeName = "varchar(64)")]
        public string Subject { get; set; }

        /// <summary>
        /// 商品描述信息
        /// </summary>
        [Comment("商品描述信息")]
        [Required, Column("body", TypeName = "varchar(256)")]
        public string Body { get; set; }

        /// <summary>
        /// 卖家备注
        /// </summary>
        [Comment("卖家备注")]
        [Column("seller_remark", TypeName = "varchar(256)")]
        public string SellerRemark { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        [Comment("买家备注")]
        [Column("buyer_remark", TypeName = "varchar(256)")]
        public string BuyerRemark { get; set; }

        /// <summary>
        /// 渠道商户号
        /// </summary>
        [Comment("渠道商户号")]
        [Column("channel_mch_no", TypeName = "varchar(64)")]
        public string ChannelMchNo { get; set; }

        /// <summary>
        /// 渠道服务商机构号
        /// </summary>
        [Comment("渠道服务商机构号")]
        [Column("channel_isv_no", TypeName = "varchar(64)")]
        public string ChannelIsvNo { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        [Comment("特定渠道发起额外参数")]
        [Column("channel_extra", TypeName = "varchar(512)")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 渠道用户标识,如微信openId,支付宝账号
        /// </summary>
        [Comment("渠道用户标识,如微信openId,支付宝账号")]
        [Column("channel_user", TypeName = "varchar(64)")]
        public string ChannelUser { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        [Comment("渠道订单号")]
        [Column("channel_order_no", TypeName = "varchar(64)")]
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 用户支付凭证交易单号 微信/支付宝流水号
        /// </summary>
        [Comment("用户支付凭证交易单号 微信/支付宝流水号")]
        [Column("platform_order_no", TypeName = "varchar(64)")]
        public string PlatformOrderNo { get; set; }

        /// <summary>
        /// 用户支付凭证商户单号
        /// </summary>
        [Comment("用户支付凭证商户单号")]
        [Column("platform_mch_order_no", TypeName = "varchar(64)")]
        public string PlatformMchOrderNo { get; set; }

        /// <summary>
        /// 退款状态: 0-未发生实际退款, 1-部分退款, 2-全额退款
        /// </summary>
        [Comment("退款状态: 0-未发生实际退款, 1-部分退款, 2-全额退款")]
        [Required, Column("refund_state", TypeName = "tinyint(6)")]
        public byte RefundState { get; set; }

        /// <summary>
        /// 退款次数
        /// </summary>
        [Comment("退款次数")]
        [Required, Column("refund_times", TypeName = "int(11)")]
        public int RefundTimes { get; set; }

        /// <summary>
        /// 退款总金额,单位分
        /// </summary>
        [Comment("退款总金额,单位分")]
        [Required, Column("refund_amount", TypeName = "bigint(20)")]
        public long RefundAmount { get; set; }

        /// <summary>
        /// 订单分账模式：0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额)
        /// </summary>
        [Comment("订单分账模式：0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额)")]
        [Column("division_mode", TypeName = "tinyint(6)")]
        public byte? DivisionMode { get; set; }

        /// <summary>
        /// 0-未发生分账, 1-等待分账任务处理, 2-分账处理中, 3-分账任务已结束(不体现状态)
        /// </summary>
        [Comment("0-未发生分账, 1-等待分账任务处理, 2-分账处理中, 3-分账任务已结束(不体现状态)")]
        [Column("division_state", TypeName = "tinyint(6)")]
        public byte? DivisionState { get; set; }

        /// <summary>
        /// 最新分账时间
        /// </summary>
        [Comment("最新分账时间")]
        [Column("division_last_time", TypeName = "datetime")]
        public DateTime? DivisionLastTime { get; set; }

        /// <summary>
        /// 渠道支付错误码
        /// </summary>
        [Comment("渠道支付错误码")]
        [Column("err_code", TypeName = "varchar(128)")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道支付错误描述
        /// </summary>
        [Comment("渠道支付错误描述")]
        [Column("err_msg", TypeName = "varchar(256)")]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        [Comment("商户扩展参数")]
        [Column("ext_param", TypeName = "varchar(128)")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        [Comment("异步通知地址")]
        [Required, Column("notify_url", TypeName = "varchar(128)")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 页面跳转地址
        /// </summary>
        [Comment("页面跳转地址")]
        [Column("return_url", TypeName = "varchar(128)")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 订单失效时间
        /// </summary>
        [Comment("订单失效时间")]
        [Column("expired_time", TypeName = "datetime")]
        public DateTime? ExpiredTime { get; set; }

        /// <summary>
        /// 订单支付成功时间
        /// </summary>
        [Comment("订单支付成功时间")]
        [Column("success_time", TypeName = "datetime")]
        public DateTime? SuccessTime { get; set; }
    }
}
