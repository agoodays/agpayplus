using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay
{
    /// <summary>
    /// 银联商务回调
    /// </summary>
    public class UmsPayChannelNoticeService : AbstractChannelNoticeService
    {
        public UmsPayChannelNoticeService(ILogger<UmsPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public UmsPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}
