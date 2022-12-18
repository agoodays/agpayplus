using AGooday.AgPay.Domain.Events.MchInfos;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class MchInfoEventHandler :
        INotificationHandler<MchInfoCreatedEvent>
    {
        public Task Handle(MchInfoCreatedEvent notification, CancellationToken cancellationToken)
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
