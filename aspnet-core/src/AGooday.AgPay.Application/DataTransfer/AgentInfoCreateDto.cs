namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 代理商信息表
    /// </summary>
    public class AgentInfoCreateDto : AgentInfoDto
    {
        /// <summary>
        /// 初始用户ID（创建代理商时，允许代理商登录的用户）
        /// </summary>
        public string LoginUsername { get; set; }

        /// <summary>
        /// 密码类型
        /// </summary>
        public string PasswordType { get; set; }

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
