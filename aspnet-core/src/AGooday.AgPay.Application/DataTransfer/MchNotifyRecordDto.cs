namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户通知记录表
    /// </summary>
    public class MchNotifyRecordDto
    {
        /// <summary>
        /// 商户通知记录ID
        /// </summary>
        public long NotifyId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 订单类型:1-支付,2-退款
        /// </summary>
        public byte OrderType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 通知请求方法
        /// </summary>
        public string ReqMethod { get; set; }

        /// <summary>
        /// 通知请求媒体类型
        /// </summary>
        public string ReqMediaType { get; set; }

        /// <summary>
        /// 通知请求正文
        /// </summary>
        public string ReqBody { get; set; }

        /// <summary>
        /// 通知响应结果
        /// </summary>
        public string ResResult { get; set; }

        /// <summary>
        /// 通知次数
        /// </summary>
        public int NotifyCount { get; set; }

        /// <summary>
        /// 最大通知次数, 默认6次
        /// </summary>
        public int NotifyCountLimit { get; set; }

        /// <summary>
        /// 通知状态,1-通知中,2-通知成功,3-通知失败
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 最后一次通知时间
        /// </summary>
        public DateTime LastNotifyTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
