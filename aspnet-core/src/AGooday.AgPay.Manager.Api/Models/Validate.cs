namespace AGooday.AgPay.Manager.Api.Models
{
    public class Validate
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Ia { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Vc { get; set; }
        /// <summary>
        /// 验证码token
        /// </summary>
        public string Vt { get; set; }
    }
}
