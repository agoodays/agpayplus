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
    }
}
