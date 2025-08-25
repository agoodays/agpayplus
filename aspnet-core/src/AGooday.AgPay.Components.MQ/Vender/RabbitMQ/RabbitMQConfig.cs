namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ
{
    public class RabbitMQConfig
    {
        /// <summary>
        /// 扇形交换机前缀（activeMQ中的topic模式）， 需根据queue动态拼接
        /// </summary>
        public const string FANOUT_EXCHANGE_NAME_PREFIX = "agpay.fanout.exchange.";

        /// <summary>
        /// 全局定义延迟交换机名称
        /// </summary>
        public const string DELAYED_EXCHANGE_NAME = "agpay.delayed.exchange";

        public MQConfig MQ { get; set; } = new MQConfig();

        public class MQConfig
        {
            public string HostName { get; set; } = "localhost";
            public int? Port { get; set; } = 5672;
            public string UserName { get; set; } = "guest";
            public string Password { get; set; } = "guest";
            public ushort? PrefetchCount { get; set; } = 10; // 默认预取数量
        }
    }
}
