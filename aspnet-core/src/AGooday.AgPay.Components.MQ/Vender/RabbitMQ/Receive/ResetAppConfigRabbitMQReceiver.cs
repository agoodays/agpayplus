using AGooday.AgPay.Components.MQ.Constants;
using AGooday.AgPay.Components.MQ.Models;

namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive
{
    /// <summary>
    /// rabbitMQ消息接收器：仅在vender=rabbitMQ时 && 项目实现IMQReceiver接口时 进行实例化
    /// 业务：更新系统配置参数
    /// </summary>
    [RabbitMQReceiver]
    public class ResetAppConfigRabbitMQReceiver : IMQMsgReceiver
    {
        private ResetAppConfigMQ.IMQReceiver mqReceiver;

        public ResetAppConfigRabbitMQReceiver(ResetAppConfigMQ.IMQReceiver mqReceiver)
        {
            this.mqReceiver = mqReceiver;
        }

        public MQSendTypeEnum GetMQType() => ResetAppConfigMQ.MQ_TYPE;

        public string GetMQName() => ResetAppConfigMQ.MQ_NAME;

        /// <summary> 
        /// 接收 【 MQSendTypeEnum.BROADCAST  】 广播类型的消息
        /// 
        /// 注意：
        /// RabbitMQ的广播模式（fanout）交换机 --》全部的Queue
        /// 如果queue包含多个消费者， 【例如，manager和payment的监听器是名称相同的queue下的消费者（Consumers） 】， 两个消费者是工作模式且存在竞争关系， 导致只能一个来消费。
        /// 解决：
        /// 每个topic的QUEUE都声明一个FANOUT交换机， 消费者声明一个系统产生的【随机队列】绑定到这个交换机上，然后往交换机发消息，只要绑定到这个交换机上都能收到消息。
        /// 参考： https://bbs.csdn.net/topics/392509262?list=70088931
        /// </summary>
        /// <param name="msg"></param>
        public Task ReceiveMsgAsync(string msg)
        {
            return mqReceiver.ReceiveAsync(ResetAppConfigMQ.Parse(msg));
        }
    }
}
