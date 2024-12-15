using System.Net;
using System.Net.Mime;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 实现回调接口抽象类
    /// </summary>
    public abstract class AbstractTransferNoticeService : ITransferNoticeService
    {
        protected readonly ILogger<AbstractTransferNoticeService> _logger;
        protected readonly RequestKit _requestKit;
        protected readonly ChannelCertConfigKit _channelCertConfigKit;
        protected readonly ConfigContextQueryService _configContextQueryService;

        protected AbstractTransferNoticeService(ILogger<AbstractTransferNoticeService> logger,
            RequestKit requestKit,
            ChannelCertConfigKit channelCertConfigKit,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _requestKit = requestKit;
            _channelCertConfigKit = channelCertConfigKit;
            _configContextQueryService = configContextQueryService;
        }

        protected AbstractTransferNoticeService()
        {
        }

        public abstract string GetIfCode();

        public abstract Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId);

        public abstract Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object parameters, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext);

        public virtual ActionResult DoNotifyOrderNotExists(HttpRequest request)
        {
            return TextResp("order not exists");
        }

        /// <summary>
        /// 文本类型的响应数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected ActionResult TextResp(string text, int statusCode = (int)HttpStatusCode.OK)
        {
            var response = new ContentResult
            {
                Content = text,
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = statusCode
            };
            return response;
        }

        protected ActionResult JsonResp(object body, int statusCode = (int)HttpStatusCode.OK)
        {
            var response = new ContentResult
            {
                Content = JObject.FromObject(body).ToString(),
                ContentType = MediaTypeNames.Application.Json,
                StatusCode = statusCode
            };
            return response;
        }

        protected Task<JObject> GetReqParamJSONAsync()
        {
            return _requestKit.GetReqParamJSONAsync();
        }

        protected Task<string> GetReqParamFromBodyAsync()
        {
            return _requestKit.GetReqParamFromBodyAsync();
        }

        protected string GetCertFilePath(string certFilePath)
        {
            return ChannelCertConfigKit.GetCertFilePath(certFilePath);
        }

        protected FileInfo GetCertFile(string certFilePath)
        {
            return ChannelCertConfigKit.GetCertFile(certFilePath);
        }
    }
}
