using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    public class AliPayDivisionService : IDivisionService
    {
        public ChannelRetMsg Bind(MchDivisionReceiverDto mchDivisionReceiver, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }

        public string GetIfCode()
        {
            throw new NotImplementedException();
        }

        public bool IsSupport()
        {
            throw new NotImplementedException();
        }

        public ChannelRetMsg SingleDivision(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }
    }
}
