using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Channel
{
    /// <summary>
    /// 转账订单通知解析实现 异步回调
    /// </summary>
    public interface ITransferNoticeService
    {
        /// <summary>
        /// 获取接口代码
        /// </summary>
        string GetIfCode();

        /// <summary>
        /// 解析参数：转账单号和请求参数
        /// 异常需要自行捕捉，并返回 null，表示已响应数据。
        /// </summary>
        /// <param name="request">HTTP 请求</param>
        /// <param name="urlOrderId">转账单号</param>
        /// <returns>转账单号和请求参数的键值对</returns>
        Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId);

        /// <summary>
        /// 执行通知处理
        /// </summary>
        /// <param name="request">HTTP 请求</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="transferOrder">转账订单</param>
        /// <param name="mchAppConfigContext">商户应用配置上下文</param>
        /// <returns>需要更新的订单状态和响应数据</returns>
        ChannelRetMsg DoNotice(HttpRequest request, object parameters, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext);

        /// <summary>
        /// 数据库订单数据不存在（仅用于异步通知）
        /// </summary>
        /// <param name="request">HTTP 请求</param>
        /// <returns>响应实体</returns>
        ActionResult DoNotifyOrderNotExists(HttpRequest request);
    }
}
