using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    /// <summary>
    /// 微信查单接口
    /// </summary>
    public class WxPayPayOrderCloseService : IPayOrderCloseService
    {
        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }
    }
}
