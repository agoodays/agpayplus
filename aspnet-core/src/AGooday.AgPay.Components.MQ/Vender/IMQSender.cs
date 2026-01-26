using AGooday.AgPay.Components.MQ.Models;

namespace AGooday.AgPay.Components.MQ.Vender
{
    /// <summary>
    /// MQ 消息发送器 接口定义
    /// </summary>
    public interface IMQSender
    {
        /// <summary>
        /// 推送MQ消息， 实时
        /// </summary>
        /// <param name="mqModel"></param>
        Task SendAsync(AbstractMQ mqModel);

        /// <summary>
        /// 推送MQ消息， 延迟接收，单位：s
        /// </summary>
        /// <param name="mqModel"></param>
        /// <param name="delay"></param>
        Task SendAsync(AbstractMQ mqModel, int delay);

        /// <summary>
        /// MQ消息接收
        /// </summary>
        Task ReceiveAsync();

        Task CloseAsync();

        /// <summary>
        /// 检查 MQ 可用性，返回 true 表示可用
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}
