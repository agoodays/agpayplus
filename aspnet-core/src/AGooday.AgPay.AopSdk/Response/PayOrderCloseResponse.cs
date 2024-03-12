using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 支付查单响应实现
    /// </summary>
    public class PayOrderCloseResponse : AgPayResponse
    {
        public PayOrderCloseResModel Get()
        {
            if (Data == null) return new PayOrderCloseResModel();
            return Data.ToObject<PayOrderCloseResModel>();
        }
    }
}
