using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 发起分账响应实现
    /// </summary>
    public class PayOrderDivisionExecResponse : AgPayResponse
    {
        public PayOrderDivisionExecResModel Get()
        {
            if (data == null) return new PayOrderDivisionExecResModel();
            return data.ToObject<PayOrderDivisionExecResModel>();
        }

        public override bool IsSuccess(string signType, string apiKey)
        {
            if (base.IsSuccess(signType, apiKey))
            {
                int state = Get().State;
                return state == 1;
            }
            return false;
        }
    }
}
