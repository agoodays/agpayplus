using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付方式表
    /// </summary>
    public class PayWayUsableQueryDto : PayWayQueryDto
    {
        /// <summary>
        /// 支付方式代码  例如： wxpay_jsapi
        /// </summary>
        public string ConfigMode { get; set; }

        /// <summary>
        /// 支付方式代码  例如： wxpay_jsapi
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 支付方式代码  例如： wxpay_jsapi
        /// </summary>
        public string IfCode { get; set; }
    }
}
