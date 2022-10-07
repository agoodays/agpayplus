
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 APP支付
    /// </summary>
    public class AliApp : AliPayPaymentService
    {
        private readonly ConfigContextQueryService _configContextQueryService;
        public AliApp(IServiceProvider serviceProvider,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider)
        {
            _configContextQueryService = configContextQueryService;
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            //统一放置 isv接口必传信息

            string payData = null;

            // sdk方式需自行拦截接口异常信息

            // 构造函数响应数据
            AliAppOrderRS res = ApiResBuilder.BuildSuccess<AliAppOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            //放置 响应数据
            channelRetMsg.ChannelAttach = payData;
            channelRetMsg.ChannelState = ChannelState.WAITING;
            res.PayData = payData;
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
