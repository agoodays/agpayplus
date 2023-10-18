using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Events.MchInfos;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class MchInfoEventHandler :
        INotificationHandler<MchInfoCreatedEvent>
    {
        public Task Handle(MchInfoCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.IsNotify.Equals(CS.YES))
            {
                // 发送开通提醒
            }
            return Task.CompletedTask;
        }
    }
}
