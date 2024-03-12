using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 退款查单响应实现
    /// </summary>
    public class RefundOrderQueryResponse : AgPayResponse
    {
        public RefundOrderQueryResModel Get()
        {
            if (Data == null) return new RefundOrderQueryResModel();
            return Data.ToObject<RefundOrderQueryResModel>();
        }
    }
}
