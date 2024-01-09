namespace AGooday.AgPay.Components.SMS.Models
{
    public class AliyundySmsConfig : AbstractSmsConfig
    {
        /// <summary>
        /// Endpoint 请参考 https://api.aliyun.com/product/Dysmsapi
        /// </summary>
        public string Endpoint { get; set; } = "dysmsapi.aliyuncs.com";
        /// <summary>
        /// id
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public string AccessKeySecret { get; set; }
        /// <summary>
        /// 签名串
        /// </summary>
        public string SignName { get; set; }
        /// <summary>
        /// 忘记密码模板ID
        /// </summary>
        public string ForgetPwdTemplateId { get; set; }
        /// <summary>
        /// 商户注册模板ID
        /// </summary>
        public string RegisterMchTemplateId { get; set; }
        /// <summary>
        /// 商户登录模板ID
        /// </summary>
        public string LoginMchTemplateId { get; set; }
        /// <summary>
        /// 账号开通提模板ID
        /// </summary>
        public string AccountOpenTemplateId { get; set; }
        /// <summary>
        /// 会员手机号绑定模板ID
        /// </summary>
        public string MbrTelBindTemplateId { get; set; }
    }
}
