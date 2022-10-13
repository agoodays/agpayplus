using AGooday.AgPay.Components.MQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ
{
    public class RabbitMQSender : IMQSender
    {
        public void Send(AbstractMQ mqModel)
        {
            throw new NotImplementedException();
        }

        public void Send(AbstractMQ mqModel, int delay)
        {
            throw new NotImplementedException();
        }
    }
}
