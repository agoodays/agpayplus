using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Models
{
    public class PayPalWrapper
    {
        public ChannelRetMsg ProcessOrder(string token, PayOrderDto payOrder)
        {
            return ProcessOrder(token, payOrder, false);
        }
        public List<string> processOrder(string order)
        {
            throw new NotImplementedException();
        }

        public ChannelRetMsg ProcessOrder(string token, PayOrderDto payOrder, bool isCapture)
        {
            throw new NotImplementedException();
        }

        public static PayPalWrapper BuildPaypalWrapper(PpPayNormalMchParams ppPayMchParams)
        {
            throw new NotImplementedException();
        }
    }
}
