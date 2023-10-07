using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay
{
    /// <summary>
    /// 银联商务 退款回调接口实现类
    /// </summary>
    public class UmsPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public UmsPayChannelRefundNoticeService(ILogger<AbstractChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.YSFPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelRefundNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelRefundNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}
