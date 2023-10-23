using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Payment.Api.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Payment.Api.Channel
{
    public abstract class AbstractChannelRefundNoticeService : IChannelRefundNoticeService
    {
        protected readonly ILogger<AbstractChannelRefundNoticeService> log;
        protected readonly RequestKit _requestKit;
        protected ConfigContextQueryService configContextQueryService;

        protected AbstractChannelRefundNoticeService(ILogger<AbstractChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
        {
            this.log = logger;
            _requestKit = requestKit;
            this.configContextQueryService = configContextQueryService;
        }

        public ActionResult DoNotifyOrderNotExists(HttpRequest request)
        {
            return TextResp("order not exists");
        }

        public ActionResult DoNotifyOrderStateUpdateFail(HttpRequest request)
        {
            return TextResp("update status error");
        }

        /// <summary>
        /// 文本类型的响应数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected ContentResult TextResp(string text)
        {
            return new ContentResult
            {
                StatusCode = 200,
                Content = text,
                ContentType = "text/html"
            };
        }

        /// <summary>
        /// json类型的响应数据
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected ObjectResult JsonResp(object body)
        {
            var mediaType = new MediaTypeCollection();
            mediaType.Add(MediaTypeHeaderValue.Parse("application/json"));
            return new ObjectResult(body)
            {
                StatusCode = 200,
                ContentTypes = mediaType
            };
        }

        protected JObject GetReqParamJSON()
        {
            return _requestKit.GetReqParamJSON();
        }

        protected string GetReqParamFromBody()
        {
            return _requestKit.GetReqParamFromBody();
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

        public abstract string GetIfCode();
        public abstract Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum);
        public abstract ChannelRetMsg DoNotice(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum);
    }
}
