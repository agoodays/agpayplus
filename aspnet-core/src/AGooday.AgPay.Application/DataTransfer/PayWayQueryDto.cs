using AGooday.AgPay.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付方式表
    /// </summary>
    public class PayWayQueryDto : PageQuery
    {
        /// <summary>
        /// 支付方式代码  例如： wxpay_jsapi
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string WayName { get; set; }
    }
}
