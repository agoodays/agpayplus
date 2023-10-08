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
            if (data == null) return new RefundOrderQueryResModel();
            return data.ToObject<RefundOrderQueryResModel>();
        }
    }
}
