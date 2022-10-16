using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Channel.PpPay
{
    public class PpPayPayOrderQueryService : IPayOrderQueryService
    {
        public string GetIfCode()
        {
            return CS.IF_CODE.PPPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return mchAppConfigContext.GetPaypalWrapper().ProcessOrder(null, payOrder);
        }
    }
}
