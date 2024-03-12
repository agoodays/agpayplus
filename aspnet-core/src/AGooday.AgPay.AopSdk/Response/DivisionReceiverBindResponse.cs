using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 分账账号的绑定响应实现
    /// </summary>
    public class DivisionReceiverBindResponse : AgPayResponse
    {
        public DivisionReceiverBindResModel Get()
        {
            if (Data == null) return new DivisionReceiverBindResModel();
            return Data.ToObject<DivisionReceiverBindResModel>();
        }

        public override bool IsSuccess(string signType, string apiKey)
        {
            if (base.IsSuccess(signType, apiKey))
            {
                int state = Get().BindState;
                return state == 1;
            }
            return false;
        }
    }
}
