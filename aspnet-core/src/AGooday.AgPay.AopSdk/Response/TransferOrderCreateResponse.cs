using AGooday.AgPay.AopSdk.Models;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// 转账响应实现
    /// </summary>
    public class TransferOrderCreateResponse : AgPayResponse
    {
        public TransferOrderCreateResModel Get()
        {
            if (Data == null) return new TransferOrderCreateResModel();
            return Data.ToObject<TransferOrderCreateResModel>();
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
