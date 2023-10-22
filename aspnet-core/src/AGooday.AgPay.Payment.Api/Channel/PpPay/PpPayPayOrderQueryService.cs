using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.PpPay
{
    public class PpPayPayOrderQueryService : IPayOrderQueryService
    {
        protected readonly ConfigContextQueryService _configContextQueryService;

        public PpPayPayOrderQueryService(ConfigContextQueryService configContextQueryService)
        {
            _configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.PPPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            return _configContextQueryService.GetPaypalWrapper(mchAppConfigContext).ProcessOrder(null, payOrder);
        }
    }
}
