using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay
{
    /// <summary>
    /// 随行付 退款回调接口实现类
    /// </summary>
    public class SxfPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public SxfPayChannelRefundNoticeService(ILogger<AbstractChannelRefundNoticeService> logger, 
            ConfigContextQueryService configContextQueryService) 
            : base(logger, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.SXFPAY;
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, IChannelRefundNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, IChannelRefundNoticeService.NoticeTypeEnum noticeTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}
