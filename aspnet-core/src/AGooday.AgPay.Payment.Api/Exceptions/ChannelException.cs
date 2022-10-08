using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Exceptions
{
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

        public ChannelException(string errorMessage, ChannelRetMsg channelRetMsg)
            : base(errorMessage)
        {
            ChannelRetMsg = channelRetMsg;
        }
    }
}
