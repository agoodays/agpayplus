namespace AGooday.AgPay.Domain.Events.SysUsers
{
    public class SysUserCreatedEvent : SysUserEvent
    {

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
