using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 支付下单响应实现
    /// </summary>
    public class PayOrderCreateResponse : AgPayResponse
    {

        public PayOrderCreateResModel Get()
        {
            if (data == null) return new PayOrderCreateResModel();
            return data.ToObject<PayOrderCreateResModel>();
        }

        public override bool IsSuccess(string signType, string apiKey)
        {
            if (base.IsSuccess(signType, apiKey))
            {
                int orderState = Get().OrderState;
                return orderState == 0 || orderState == 1 || orderState == 2;
            }
            return false;
        }
    }
}
