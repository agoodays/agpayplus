using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay
{
    /// <summary>
    /// 银联商务回调
    /// </summary>
    public class UmsPayChannelNoticeService : AbstractChannelNoticeService
    {
        public UmsPayChannelNoticeService(ILogger<AbstractChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}
