using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 转账订单表
    /// </summary>
    public class TransferOrderQueryDto : DatePageQuery
    {
        /// <summary>
        /// 转账订单号
        /// </summary>
        public string TransferId { get; set; }

        /// <summary>
        /// 三合一订单
        /// </summary>
        public string UnionOrderId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [BindNever]
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        [BindNever]
        public byte? MchType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [BindNever]
        public string IfCode { get; set; }

        /// <summary>
        /// 入账方式： WX_CASH-微信零钱; ALIPAY_CASH-支付宝转账; BANK_CARD-银行卡
        /// </summary>
        public string EntryType { get; set; }

        /// <summary>
        /// 转账金额,单位分
        /// </summary>
        [BindNever]
        public long Amount { get; set; }

        /// <summary>
        /// 三位货币代码, 人民币: CNY
        /// </summary>
        [BindNever]
        public string Currency { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        [BindNever]
        public string AccountNo{ get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        [BindNever]
        public string AccountName{ get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        [BindNever]
        public string BankName{ get; set; }

        /// <summary>
        /// 转账备注信息
        /// </summary>
        [BindNever]
        public string TransferDesc{ get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [BindNever]
        public string ClientIp{ get; set; }

        /// <summary>
        /// 支付状态: 0-订单生成, 1-转账中, 2-转账成功, 3-转账失败, 4-订单关闭
        /// </summary>
        public byte State{ get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        [BindNever]
        public string ChannelExtra{ get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderNo{ get; set; }

        /// <summary>
        /// 渠道支付错误码
        /// </summary>
        [BindNever]
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道支付错误描述
        /// </summary>
        [BindNever]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        [BindNever]
        public string ExtParam { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        [BindNever]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 转账成功时间
        /// </summary>
        [BindNever]
        public DateTime SuccessTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BindNever]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [BindNever]
        public DateTime UpdatedAt { get; set; }
    }
}
