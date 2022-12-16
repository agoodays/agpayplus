using AGooday.AgPay.Domain.Events.AgentInfos;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class AgentInfoEventHandler :
        INotificationHandler<AgentInfoCreatedEvent>
    {
        public Task Handle(AgentInfoCreatedEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
