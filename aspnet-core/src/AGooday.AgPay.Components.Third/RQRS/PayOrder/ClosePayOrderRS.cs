using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder
{
    public class ClosePayOrderRS : AbstractRS
    {
        [JsonIgnore]
        public ChannelRetMsg ChannelRetMsg { get; set; }
    }
}
