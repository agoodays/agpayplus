namespace AGooday.AgPay.Agent.Api.Models
{
    public class Validate
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string ia { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string vc { get; set; }
        /// <summary>
        /// 验证码token
        /// </summary>
        public string vt { get; set; }
        /// <summary>
        /// 登录类型： APP-app登录， lite-小程序登录
        /// </summary>
        public string lt { get; set; }
    }
}
