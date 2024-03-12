using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 支付查单响应实现
    /// </summary>
    public class PayOrderQueryResponse : AgPayResponse
    {
        public PayOrderQueryResModel Get()
        {
            if (Data == null) return new PayOrderQueryResModel();
            return Data.ToObject<PayOrderQueryResModel>();
        }
    }
}
