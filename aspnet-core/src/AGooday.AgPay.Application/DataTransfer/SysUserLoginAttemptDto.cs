namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统用户登录尝试记录表
    /// </summary>
    public class SysUserLoginAttemptDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 登录类型  1-昵称 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博
        /// </summary>
        public byte IdentityType { get; set; }

        /// <summary>
        /// 认证标识 ( 用户名 | open_id )
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 登录成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 尝试时间
        /// </summary>
        public DateTime AttemptTime { get; set; }
    }
}
