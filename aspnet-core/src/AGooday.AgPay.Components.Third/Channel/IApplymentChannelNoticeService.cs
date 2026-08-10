using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 进件回调通知接口（参考 IChannelNoticeService 模式）
    ///
    /// 与支付订单通知的关键差异：
    ///   1. 只有异步通知（DO_NOTIFY），没有同步跳转（DO_RETURN）
    ///   2. 上下文为 IsvConfigContext（服务商模式），而非 MchAppConfigContext
    ///   3. 业务实体为 MchApplyDto，而非 PayOrderDto
    /// </summary>
    public interface IApplymentChannelNoticeService
    {
        enum NoticeTypeEnum
        {
            DO_NOTIFY //异步回调
        }

        /// <summary>获取通道接口 code</summary>
        string GetIfCode();

        /// <summary>
        /// 解析参数：返回字典，key 为 applyId（或 channelApplyNo），value 为解析后的请求数据
        /// 解析失败返回 null，响应已由实现类自行处理
        /// </summary>
        Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlApplyId, NoticeTypeEnum noticeTypeEnum);

        /// <summary>
        /// 处理回调业务逻辑，返回需要更新的进件状态 + 响应数据
        /// </summary>
        Task<ApplymentNoticeResult> DoNoticeAsync(HttpRequest request, object @params, MchApplyDto mchApply, IsvConfigContext isvConfigContext, NoticeTypeEnum noticeTypeEnum);

        /// <summary>进件单不存在（回调找不到记录）时的响应</summary>
        ActionResult DoNotifyApplyNotExists(HttpRequest request);

        /// <summary>DB 状态更新异常时的响应</summary>
        ActionResult DoNotifyApplyStateUpdateFail(HttpRequest request);
    }

    /// <summary>
    /// 进件回调处理结果（对应支付的 ChannelRetMsg）
    /// </summary>
    public class ApplymentNoticeResult
    {
        /// <summary>渠道侧状态</summary>
        public ApplymentNoticeState State { get; set; }

        /// <summary>渠道商户号（审核通过后返回）</summary>
        public string ChannelMchId { get; set; }

        /// <summary>渠道进件申请单号</summary>
        public string ChannelApplyNo { get; set; }

        /// <summary>错误码</summary>
        public string ErrCode { get; set; }

        /// <summary>错误描述</summary>
        public string ErrMsg { get; set; }

        /// <summary>签名链接</summary>
        public string SignUrl { get; set; }

        /// <summary>验签金额（分）</summary>
        public long? VerifyAmount { get; set; }

        /// <summary>渠道原始响应 JSON</summary>
        public string ChannelOriginResponse { get; set; }

        /// <summary>返回给渠道的 HTTP 响应</summary>
        public ActionResult ResponseEntity { get; set; }

        public static ApplymentNoticeResult ConfirmSuccess(string channelMchId = null)
            => new() { State = ApplymentNoticeState.CONFIRM_SUCCESS, ChannelMchId = channelMchId };

        public static ApplymentNoticeResult ConfirmFail(string errCode, string errMsg)
            => new() { State = ApplymentNoticeState.CONFIRM_FAIL, ErrCode = errCode, ErrMsg = errMsg };

        public static ApplymentNoticeResult Waiting()
            => new() { State = ApplymentNoticeState.WAITING };
    }

    /// <summary>
    /// 进件回调状态（对应支付的 ChannelState）
    /// </summary>
    public enum ApplymentNoticeState
    {
        /// <summary>明确成功（审核通过）</summary>
        CONFIRM_SUCCESS,
        /// <summary>明确失败（审核拒绝）</summary>
        CONFIRM_FAIL,
        /// <summary>处理中（等待后续通知 / 轮询）</summary>
        WAITING,
        /// <summary>状态未知（网络异常 / 签名失败）</summary>
        UNKNOWN,
        /// <summary>渠道接口返回异常状态</summary>
        API_RET_ERROR,
        /// <summary>系统内部异常</summary>
        SYS_ERROR,
    }
}
