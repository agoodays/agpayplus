namespace AGooday.AgPay.Base.Api.Models
{
    public class SmsCode
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 短信验证码类型
        /// </summary>
        public string SmsType { get; set; }
    }
}
