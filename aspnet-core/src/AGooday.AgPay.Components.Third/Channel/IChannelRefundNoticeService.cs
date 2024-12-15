using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 渠道侧的退款订单通知解析实现 【分为同步跳转（doReturn）和异步回调(doNotify) 】
    /// </summary>
    public interface IChannelRefundNoticeService
    {
        /// <summary>
        /// 通知类型
        /// </summary>
        enum NoticeTypeEnum
        {
            DO_NOTIFY //异步回调
        }

        /// <summary>
        /// 获取到接口code
        /// </summary>
        /// <returns></returns>
        string GetIfCode();

        /// <summary>
        /// 解析参数： 订单号 和 请求参数
        /// 异常需要自行捕捉，并返回null , 表示已响应数据。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="urlOrderId"></param>
        /// <param name="noticeTypeEnum"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum);

        /// <summary>
        /// 返回需要更新的订单状态 和响应数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="params"></param>
        /// <param name="refundOrder"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <param name="noticeTypeEnum"></param>
        /// <returns></returns>
        Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum);

        /// <summary>
        /// 数据库订单 状态更新异常 (仅异步通知使用)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ActionResult DoNotifyOrderStateUpdateFail(HttpRequest request);

        /// <summary>
        /// 数据库订单数据不存在  (仅异步通知使用)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ActionResult DoNotifyOrderNotExists(HttpRequest request);
    }
}
