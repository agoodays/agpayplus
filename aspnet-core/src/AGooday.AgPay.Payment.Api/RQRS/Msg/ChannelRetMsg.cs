using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.RQRS.Msg
{
    /// <summary>
    /// 上游渠道侧响应信息包装类
    /// </summary>
    public class ChannelRetMsg
    {
        /// <summary>
        /// 上游渠道返回状态
        /// </summary>
        public ChannelState? ChannelState { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderId { get; set; }

        /// <summary>
        /// 渠道用户标识
        /// </summary>
        public string ChannelUserId { get; set; }

        /// <summary>
        /// 用户支付凭证交易单号 微信/支付宝流水号
        /// </summary>
        public string PlatformOrderId { get; set; }

        /// <summary>
        /// 用户支付凭证商户单号
        /// </summary>
        public string PlatformMchOrderId { get; set; }

        /// <summary>
        /// 渠道错误码
        /// </summary>
        public string ChannelErrCode { get; set; }

        /// <summary>
        /// 渠道错误描述
        /// </summary>
        public string ChannelErrMsg { get; set; }

        /// <summary>
        /// 渠道支付数据包, 一般用于支付订单的继续支付操作
        /// </summary>
        public string ChannelAttach { get; set; }

        /// <summary>
        /// 上游渠道返回的原始报文, 一般用于[运营平台的查询上游结果]功能
        /// </summary>
        public string ChannelOriginResponse { get; set; }

        /// <summary>
        /// 是否需要轮询查单（比如微信条码支付） 默认不查询订单
        /// </summary>
        public bool IsNeedQuery { get; set; } = false;

        /// <summary>
        /// 响应结果（一般用于回调接口返回给上游数据 ）
        /// </summary>
        public ActionResult ResponseEntity { get; set; }

        //静态初始函数
        public ChannelRetMsg() { }
        public ChannelRetMsg(ChannelState channelState, string channelOrderId, string channelErrCode, string channelErrMsg)
        {
            this.ChannelState = channelState;
            this.ChannelOrderId = channelOrderId;
            this.ChannelErrCode = channelErrCode;
            this.ChannelErrMsg = channelErrMsg;
        }

        /// <summary>
        /// 明确成功
        /// </summary>
        /// <param name="channelOrderId"></param>
        /// <returns></returns>
        public static ChannelRetMsg ConfirmSuccess(string channelOrderId)
        {
            return new ChannelRetMsg(Msg.ChannelState.CONFIRM_SUCCESS, channelOrderId, null, null);
        }

        /// <summary>
        /// 明确失败
        /// </summary>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public static ChannelRetMsg ConfirmFail(string channelErrCode, string channelErrMsg)
        {
            return new ChannelRetMsg(Msg.ChannelState.CONFIRM_FAIL, null, channelErrCode, channelErrMsg);
        }

        /// <summary>
        /// 明确失败
        /// </summary>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public static ChannelRetMsg ConfirmFail(string channelOrderId, string channelErrCode, string channelErrMsg)
        {
            return new ChannelRetMsg(Msg.ChannelState.CONFIRM_FAIL, channelOrderId, channelErrCode, channelErrMsg);
        }

        /// <summary>
        /// 明确失败
        /// </summary>
        /// <param name="channelOrderId"></param>
        /// <returns></returns>
        public static ChannelRetMsg ConfirmFail(string channelOrderId)
        {
            return new ChannelRetMsg(Msg.ChannelState.CONFIRM_FAIL, channelOrderId, null, null);
        }

        /// <summary>
        /// 明确失败
        /// </summary>
        /// <returns></returns>
        public static ChannelRetMsg ConfirmFail()
        {
            return new ChannelRetMsg(Msg.ChannelState.CONFIRM_FAIL, null, null, null);
        }

        /// <summary>
        /// 处理中
        /// </summary>
        /// <returns></returns>
        public static ChannelRetMsg Waiting()
        {
            return new ChannelRetMsg(Msg.ChannelState.WAITING, null, null, null);
        }

        /// <summary>
        /// 异常的情况
        /// </summary>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public static ChannelRetMsg SysError(string channelErrMsg)
        {
            return new ChannelRetMsg(Msg.ChannelState.SYS_ERROR, null, null, "系统：" + channelErrMsg);
        }

        /// <summary>
        /// 状态未知的情况
        /// </summary>
        /// <returns></returns>
        public static ChannelRetMsg Unknown()
        {
            return new ChannelRetMsg(Msg.ChannelState.UNKNOWN, null, null, null);
        }

        /// <summary>
        /// 状态未知的情况
        /// </summary>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public static ChannelRetMsg Unknown(string channelErrMsg)
        {
            return new ChannelRetMsg(Msg.ChannelState.UNKNOWN, null, null, channelErrMsg);
        }
    }

    //渠道状态枚举值
    public enum ChannelState
    {
        /// <summary>
        /// 接口正确返回： 业务状态已经明确成功
        /// </summary>
        CONFIRM_SUCCESS,
        /// <summary>
        /// 接口正确返回： 业务状态已经明确失败
        /// </summary>
        CONFIRM_FAIL,
        /// <summary>
        /// 接口正确返回： 上游处理中， 需通过定时查询/回调进行下一步处理
        /// </summary>
        WAITING,
        /// <summary>
        /// 接口超时，或网络异常等请求， 或者返回结果的签名失败： 状态不明确 ( 上游接口变更, 暂时无法确定状态值 )
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// 渠道侧出现异常( 接口返回了异常状态 )
        /// </summary>
        API_RET_ERROR,
        /// <summary>
        /// 本系统出现不可预知的异常
        /// </summary>
        SYS_ERROR
    }
}
