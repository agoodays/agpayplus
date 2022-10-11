using AGooday.AgPay.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： WX_H5
    /// </summary>
    public class WxH5OrderRQ : CommonPayDataRQ
    {
        /** 构造函数 **/
        public WxH5OrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.WX_H5;
        }
    }
}
