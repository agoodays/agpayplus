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
    /// 分账结果回调接口抽象类
    /// </summary>
    public abstract class AbstractDivisionRecordChannelNotifyService : IDivisionRecordChannelNotifyService
    {
        protected readonly ILogger<AbstractDivisionRecordChannelNotifyService> _logger;
        protected readonly RequestKit _requestKit;
        protected readonly ConfigContextQueryService _configContextQueryService;

        protected AbstractDivisionRecordChannelNotifyService(ILogger<AbstractDivisionRecordChannelNotifyService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _requestKit = requestKit;
            _configContextQueryService = configContextQueryService;
        }

        protected AbstractDivisionRecordChannelNotifyService()
        {
        }

        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        public abstract string GetIfCode();

        /// <summary>
        /// 解析参数： 批次号 和 请求参数
        /// 异常需要自行捕捉，并返回null , 表示已响应数据。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Dictionary<string, object> ParseParams(HttpRequest request);

        /// <summary>
        /// 返回需要更新的记录 <ID, 结果> 状态 和响应数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="parameters"></param>
        /// <param name="recordList"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        public abstract DivisionChannelNotifyModel DoNotify(HttpRequest request, object parameters, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext);

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
        protected ActionResult TextResp(string text, int statusCode = (int)HttpStatusCode.OK)
        {
            var response = new ContentResult
            {
                Content = text,
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = statusCode // StatusCodes.Status200OK
            };
            return response;
        }

        /// <summary>
        /// json类型的响应数据
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected ActionResult JsonResp(object body, int statusCode = (int)HttpStatusCode.OK)
        {
            var response = new JsonResult(body)
            {
                StatusCode = statusCode // StatusCodes.Status200OK
            };
            return response;
        }

        /// <summary>
        /// request.getParameter 获取参数 并转换为JSON格式
        /// </summary>
        /// <returns></returns>
        protected JObject GetReqParamJSON()
        {
            return _requestKit.GetReqParamJSON();
        }

        /// <summary>
        /// request.getParameter 获取参数 并转换为JSON格式
        /// </summary>
        /// <returns></returns>
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
    }
}
