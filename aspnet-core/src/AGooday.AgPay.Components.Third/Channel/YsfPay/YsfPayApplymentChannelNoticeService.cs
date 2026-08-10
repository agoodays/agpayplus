using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AGooday.AgPay.Components.Third.Channel.IApplymentChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.YsfPay
{
    public class YsfPayApplymentChannelNoticeService : AbstractApplymentChannelNoticeService
    {
        public YsfPayApplymentChannelNoticeService(
            ILogger<YsfPayApplymentChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public YsfPayApplymentChannelNoticeService() : base() { }

        public override string GetIfCode() => CS.IF_CODE.YSFPAY;

        public override Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlApplyId, NoticeTypeEnum noticeTypeEnum)
        {
            return Task.FromResult(new Dictionary<string, object>());
        }

        public override Task<ApplymentNoticeResult> DoNoticeAsync(HttpRequest request, object @params, MchApplyDto mchApply, IsvConfigContext isvConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            var result = new ApplymentNoticeResult
            {
                State = ApplymentNoticeState.UNKNOWN,
                ResponseEntity = TextResp("not supported")
            };
            return Task.FromResult(result);
        }
    }
}
