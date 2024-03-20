using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.Transfer
{
    /// <summary>
    /// 申请转账 请求参数
    /// </summary>
    public class TransferOrderRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        [Required(ErrorMessage = "商户订单号不能为空")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Required(ErrorMessage = "支付接口代码不能为空")]
        public string IfCode { get; set; }

        /// <summary>
        /// 入账方式
        /// </summary>
        [Required(ErrorMessage = "入账方式不能为空")]
        public string EntryType { get; set; }

        /// <summary>
        /// 付金额， 单位：分
        /// </summary>
        [Required(ErrorMessage = "转账金额不能为空")]
        [Range(1, long.MaxValue, ErrorMessage = "转账金额不能小于1分")]
        public long Amount { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        [Required(ErrorMessage = "货币代码不能为空")]
        [AllowedValues("CNY", ErrorMessage = "货币代码，目前只支持人民币：CNY")]
        public string Currency { get; set; }

        /// <summary>
        /// 收款账号不能为空
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 转账备注信息
        /// </summary>
        [Required(ErrorMessage = "转账备注信息不能为空")]
        public string TransferDesc { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        public string ExtParam { get; set; }
    }
}
