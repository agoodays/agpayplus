namespace AGooday.AgPay.Notice.Core
{
    public interface INotice
    {
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<NoticeSendResponse> SendAsync(NoticeSendRequest request);
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