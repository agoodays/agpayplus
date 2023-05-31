using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Exceptions
{
    /// <summary>
    /// 请求渠道侧异常 exception
    /// 抛出此异常： 仅支持：  未知状态（需查单） 和 系统内异常
    /// </summary>
    public class ChannelException : Exception
    {
        public ChannelRetMsg ChannelRetMsg { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public ChannelException(string errorMessage)
            : base(errorMessage)
        {
        }

        /// <summary>
        /// 业务自定义异常
        /// </summary>
        /// <param name="channelRetMsg"></param>
        private ChannelException(ChannelRetMsg channelRetMsg)
            : base(channelRetMsg.ChannelErrMsg)
        {
            ChannelRetMsg = channelRetMsg;
        }

        /// <summary>
        /// 未知状态
        /// </summary>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public static ChannelException Unknown(string channelErrMsg)
        {
            return new ChannelException(ChannelRetMsg.Unknown(channelErrMsg));
        }

        /// <summary>
        /// 系统内异常
        /// </summary>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public static ChannelException SysError(string channelErrMsg)
        {
            return new ChannelException(ChannelRetMsg.SysError(channelErrMsg));
        }
    }
}
