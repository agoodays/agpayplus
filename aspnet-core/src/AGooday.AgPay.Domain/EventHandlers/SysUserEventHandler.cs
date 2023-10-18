using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Events.SysUsers;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class SysUserEventHandler :
        INotificationHandler<SysUserCreatedEvent>
    {
        public Task Handle(SysUserCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.IsNotify.Equals(CS.YES))
            {
                // 发送开通提醒
            }
            return Task.CompletedTask;
        }
    }
}
