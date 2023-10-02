using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 分账结果回调接口抽象类
    /// </summary>
    public abstract class AbstractDivisionRecordChannelNotifyService
    {
        protected readonly RequestKit _requestKit;
        protected readonly ConfigContextQueryService _configContextQueryService;

        public AbstractDivisionRecordChannelNotifyService(
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
        {
            _requestKit = requestKit;
            _configContextQueryService = configContextQueryService;
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
        protected ActionResult TextResp(string text)
        {
            var response = new ContentResult
            {
                Content = text,
                ContentType = "text/html",
                StatusCode = StatusCodes.Status200OK
            };
            return response;
        }

        /// <summary>
        /// json类型的响应数据
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected ActionResult JsonResp(object body)
        {
            var response = new JsonResult(body)
            {
                StatusCode = StatusCodes.Status200OK
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
