using System.Net;
using System.Net.Mime;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IApplymentChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 进件渠道回调抽象基类（参考 AbstractChannelNoticeService 模式）
    /// </summary>
    public abstract class AbstractApplymentChannelNoticeService : IApplymentChannelNoticeService
    {
        protected readonly ILogger<AbstractApplymentChannelNoticeService> _logger;
        protected readonly RequestKit _requestKit;
        protected ConfigContextQueryService _configContextQueryService;

        protected AbstractApplymentChannelNoticeService(ILogger<AbstractApplymentChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _requestKit = requestKit;
            _configContextQueryService = configContextQueryService;
        }

        protected AbstractApplymentChannelNoticeService() { }

        public abstract string GetIfCode();

        public abstract Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlApplyId, NoticeTypeEnum noticeTypeEnum);

        public abstract Task<ApplymentNoticeResult> DoNoticeAsync(HttpRequest request, object @params, MchApplyDto mchApply, IsvConfigContext isvConfigContext, NoticeTypeEnum noticeTypeEnum);

        public virtual ActionResult DoNotifyApplyNotExists(HttpRequest request)
            => TextResp("apply not exists");

        public virtual ActionResult DoNotifyApplyStateUpdateFail(HttpRequest request)
            => TextResp("update status error");

        protected ContentResult TextResp(string text, int statusCode = (int)HttpStatusCode.OK)
        {
            return new ContentResult
            {
                StatusCode = statusCode,
                Content = text,
                ContentType = MediaTypeNames.Text.Html,
            };
        }

        protected ObjectResult JsonResp(object body, int statusCode = (int)HttpStatusCode.OK)
        {
            var mediaType = new MediaTypeCollection();
            mediaType.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
            return new ObjectResult(body)
            {
                StatusCode = statusCode,
                ContentTypes = mediaType
            };
        }

        protected Task<JObject> GetReqParamJsonAsync() => _requestKit.GetReqParamJsonAsync();
        protected Task<string> GetReqParamFromBodyAsync() => _requestKit.GetReqParamFromBodyAsync();
    }
}
