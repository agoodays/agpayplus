using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 转账订单表
    /// </summary>
    [Comment("转账订单表")]
    [Table("t_transfer_order")]
    public class TransferOrder
    {
        /// <summary>
        /// 转账订单号
        /// </summary>
        [Comment("转账订单号")]
        [Key, Required, Column("transfer_id", TypeName = "varchar(32)")]
        public string TransferId { get; set; }

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
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 入账方式： WX_CASH-微信零钱; ALIPAY_CASH-支付宝转账; BANK_CARD-银行卡
        /// </summary>
        [Comment("入账方式： WX_CASH-微信零钱; ALIPAY_CASH-支付宝转账; BANK_CARD-银行卡")]
        [Required, Column("entry_type", TypeName = "varchar(20)")]
        public string EntryType { get; set; }

        /// <summary>
        /// 转账金额,单位分
        /// </summary>
        [Comment("转账金额,单位分")]
        [Required, Column("amount", TypeName = "bigint")]
        public long Amount { get; set; }

        /// <summary>
        /// 三位货币代码, 人民币: CNY
        /// </summary>
        [Comment("三位货币代码, 人民币: CNY")]
        [Required, Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        [Comment("收款账号")]
        [Required, Column("account_no", TypeName = "varchar(64)")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        [Comment("收款人姓名")]
        [Column("account_name", TypeName = "varchar(64)")]
        public string AccountName { get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        [Comment("收款人开户行名称")]
        [Column("bank_name", TypeName = "varchar(32)")]
        public string BankName { get; set; }

        /// <summary>
        /// 转账备注信息
        /// </summary>
        [Comment("转账备注信息")]
        [Required, Column("transfer_desc", TypeName = "varchar(128)")]
        public string TransferDesc { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [Comment("客户端IP")]
        [Column("client_ip", TypeName = "varchar(32)")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 支付状态: 0-订单生成, 1-转账中, 2-转账成功, 3-转账失败, 4-订单关闭
        /// </summary>
        [Comment("支付状态: 0-订单生成, 1-转账中, 2-转账成功, 3-转账失败, 4-订单关闭")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        [Comment("特定渠道发起额外参数")]
        [Column("channel_extra", TypeName = "varchar(512)")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        [Comment("渠道订单号")]
        [Column("channel_order_no", TypeName = "varchar(64)")]
        public string ChannelOrderNo { get; set; }

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
        /// 转账成功时间
        /// </summary>
        [Comment("转账成功时间")]
        [Column("success_time", TypeName = "datetime")]
        public DateTime? SuccessTime { get; set; }

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
