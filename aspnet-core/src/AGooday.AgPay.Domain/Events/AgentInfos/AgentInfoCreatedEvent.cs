namespace AGooday.AgPay.Domain.Events.AgentInfos
{
    public class AgentInfoCreatedEvent : AgentInfoEvent
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginUsername { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword { get; set; }

        /// <summary>
        /// 是否发送开通提醒
        /// </summary>
        public byte IsNotify { get; set; }
    }
}
