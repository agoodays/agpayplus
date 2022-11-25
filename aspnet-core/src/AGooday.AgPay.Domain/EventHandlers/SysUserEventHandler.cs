using AGooday.AgPay.Domain.Events.SysUsers;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class SysUserEventHandler :
        INotificationHandler<SysUserCreatedEvent>
    {
        public Task Handle(SysUserCreatedEvent notification, CancellationToken cancellationToken)
        {
            // 恭喜您，注册成功，欢迎加入我们。

            return Task.CompletedTask;
        }
    }
}
