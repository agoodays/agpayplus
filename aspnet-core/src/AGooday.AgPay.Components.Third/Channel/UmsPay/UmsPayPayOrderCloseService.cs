using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay
{
    /// <summary>
    /// 银联商务关单
    /// </summary>
    public class UmsPayPayOrderCloseService : IPayOrderCloseService
    {
        public string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }
        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }
    }
}
