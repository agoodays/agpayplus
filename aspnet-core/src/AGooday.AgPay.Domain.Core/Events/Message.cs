using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Core.Events
{
    /// <summary>
    /// 抽象类Message，用来获取我们事件执行过程中的类名
    /// 然后并且添加聚合根
    /// 
    /// IRequest 通过引入结构类型Unit来代表无返回的情况。
    /// </summary>
    public abstract class Message : IRequest
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
    public abstract class Message<T> : IRequest<T>
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
