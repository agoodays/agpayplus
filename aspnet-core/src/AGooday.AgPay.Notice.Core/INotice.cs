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
    }
}