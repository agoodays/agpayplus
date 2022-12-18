using AGooday.AgPay.Domain.Events.AgentInfos;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class AgentInfoEventHandler :
        INotificationHandler<AgentInfoCreatedEvent>
    {
        public Task Handle(AgentInfoCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.IsNotify.Equals(1))
            {
                // 发送开通提醒
                return Task.CompletedTask;
            }
            throw new NotImplementedException();
        }
    }
}
