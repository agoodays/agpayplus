using System.Net;
using System.Net.Mime;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel
{
    public abstract class AbstractChannelNoticeService : IChannelNoticeService
    {
        protected readonly ILogger<AbstractChannelNoticeService> _logger;
        protected readonly RequestKit _requestKit;
        protected ConfigContextQueryService _configContextQueryService;

        protected AbstractChannelNoticeService(ILogger<AbstractChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _requestKit = requestKit;
            _configContextQueryService = configContextQueryService;
        }

        protected AbstractChannelNoticeService()
        {
        }

        public abstract string GetIfCode();

        public abstract Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum);

        public abstract Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum);

        public virtual ActionResult DoNotifyOrderNotExists(HttpRequest request)
        {
            return TextResp("order not exists");
        }

        public virtual ActionResult DoNotifyOrderStateUpdateFail(HttpRequest request)
        {
            return TextResp("update status error");
        }

        /// <summary>
        /// 文本类型的响应数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected ContentResult TextResp(string text, int statusCode = (int)HttpStatusCode.OK)
        {
            return new ContentResult
            {
                StatusCode = statusCode,
                Content = text,
                ContentType = MediaTypeNames.Text.Html,
            };
        }

        /// <summary>
        /// json类型的响应数据
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
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

        protected Task<JObject> GetReqParamJSONAsync()
        {
            return _requestKit.GetReqParamJSONAsync();
        }

        protected Task<string> GetReqParamFromBodyAsync()
        {
            return _requestKit.GetReqParamFromBodyAsync();
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="certFilePath"></param>
        /// <returns></returns>
        protected string GetCertFilePath(string certFilePath)
        {
            return ChannelCertConfigKit.GetCertFilePath(certFilePath);
        }

        /// <summary>
        /// 获取文件File对象
        /// </summary>
        /// <param name="certFilePath"></param>
        /// <returns></returns>
        protected FileInfo GetCertFile(string certFilePath)
        {
            return ChannelCertConfigKit.GetCertFile(certFilePath);
        }
    }
}
