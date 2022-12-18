namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    public class MchInfoCreateDto : MchInfoDto
    {
        /// <summary>
        /// 登录名
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
    }
}
