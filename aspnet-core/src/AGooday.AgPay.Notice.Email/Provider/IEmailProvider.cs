using AGooday.AgPay.Notice.Core;

namespace AGooday.AgPay.Notice.Email
{
    public interface IEmailProvider : INotice
    {
        /// <summary>
        /// 设置发送地址
        /// </summary>
        /// <param name="toAddress"></param>
        public void SetToAddress(List<string> toAddress);

        /// <summary>
        /// 推送异常消息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        Task<NoticeSendResponse> SendAsync(string title, Exception ex);

        /// <summary>
        /// 推送正常消息
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        Task<NoticeSendResponse> SendAsync(string title, string message);
    }
}
