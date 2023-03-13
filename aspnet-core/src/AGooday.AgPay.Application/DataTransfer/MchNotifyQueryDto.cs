using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户通知记录表
    /// </summary>
    public class MchNotifyQueryDto : DatePageQuery
    {
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
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 通知状态,1-通知中,2-通知成功,3-通知失败
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? CreatedStart { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? CreatedEnd { get; set; }
    }
}
