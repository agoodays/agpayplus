using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 退款响应实现
    /// </summary>
    public class RefundOrderCreateResponse : AgPayResponse
    {
        public RefundOrderCreateResModel Get()
        {
            if (Data == null) return new RefundOrderCreateResModel();
            return Data.ToObject<RefundOrderCreateResModel>();
        }

        public override bool IsSuccess(string signType, string apiKey)
        {
            if (base.IsSuccess(signType, apiKey))
            {
                int state = Get().State;
                return state == 0 || state == 1 || state == 2;
            }
            return false;
        }
    }
}
