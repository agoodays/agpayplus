using AGooday.AgPay.Domain.Events.MchInfos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class MchInfoEventHandler :
        INotificationHandler<MchInfoCreatedEvent>
    {
        public Task Handle(MchInfoCreatedEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
