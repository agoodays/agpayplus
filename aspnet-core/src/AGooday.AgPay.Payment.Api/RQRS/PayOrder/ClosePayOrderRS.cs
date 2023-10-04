using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    public class ClosePayOrderRS : AbstractRS
    {
        [JsonIgnore]
        public ChannelRetMsg ChannelRetMsg { get; set; }
    }
}
