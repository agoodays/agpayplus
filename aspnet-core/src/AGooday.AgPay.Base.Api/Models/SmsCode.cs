namespace AGooday.AgPay.Base.Api.Models
{
    public class SmsCode
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 短信验证码类型 1.注册 2.登录 3.修改密码 4.设置支付密码 5.修改手机号
        /// </summary>
        public byte SmsType { get; set; }
    }
}
