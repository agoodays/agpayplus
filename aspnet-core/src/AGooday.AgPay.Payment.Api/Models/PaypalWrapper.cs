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

        public List<string> ProcessOrder(string order)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理并捕获订单
        /// 由于 Paypal 创建订单后需要进行一次 Capture(捕获) 才可以正确获取到订单的支付状态
        /// </summary>
        /// <param name="token"></param>
        /// <param name="payOrder"></param>
        /// <param name="isCapture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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
