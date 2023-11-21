namespace AGooday.AgPay.Agent.Api.Models
{
    public class PhoneCode
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 登录类型： WEB-web登录， APP-app登录， LITE-小程序登录
        /// </summary>
        public string lt { get; set; }
    }
}
