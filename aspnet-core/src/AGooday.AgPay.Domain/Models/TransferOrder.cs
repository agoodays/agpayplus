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
    /// 转账订单表
    /// </summary>
    [Table("t_transfer_order")]
    public class TransferOrder
    {
        /// <summary>
        /// 转账订单号
        /// </summary>
        [Key, Required, Column("refund_order_id", TypeName = "varchar(32)")]
        public string TransferId { get; set; }

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
        /// 应用ID
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
        [Required, Column("mch_type", TypeName = "tinyint(6)")]
        public byte MchType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [Required, Column("mch_order_no", TypeName = "varchar(64)")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>
        /// 入账方式： WX_CASH-微信零钱; ALIPAY_CASH-支付宝转账; BANK_CARD-银行卡
        /// </summary>
        [Required, Column("entry_type", TypeName = "varchar(20)")]
        public string EntryType { get; set; }

        /// <summary>
        /// 转账金额,单位分
        /// </summary>
        [Required, Column("amount", TypeName = "bigint")]
        public long Amount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        [Required, Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        [Required, Column("account_no", TypeName = "varchar(64)")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        [Column("account_name", TypeName = "varchar(64)")]
        public string AccountName { get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        [Column("bank_name", TypeName = "varchar(32)")]
        public string BankName { get; set; }

        /// <summary>
        /// 转账备注信息
        /// </summary>
        [Required, Column("transfer_desc", TypeName = "varchar(128)")]
        public string TransferDesc { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [Column("client_ip", TypeName = "varchar(32)")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 支付状态: 0-订单生成, 1-转账中, 2-转账成功, 3-转账失败, 4-订单关闭
        /// </summary>
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        [Column("channel_extra", TypeName = "varchar(512)")]
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        [Column("channel_order_no", TypeName = "varchar(64)")]
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道支付错误码
        /// </summary>
        [Column("err_code", TypeName = "varchar(128)")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道支付错误描述
        /// </summary>
        [Column("err_msg", TypeName = "varchar(256)")]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        [Column("ext_param", TypeName = "varchar(128)")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        [Required, Column("notify_url", TypeName = "varchar(128)")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 转账成功时间
        /// </summary>
        [Column("success_time", TypeName = "datetime")]
        public DateTime SuccessTime { get; set; }

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
