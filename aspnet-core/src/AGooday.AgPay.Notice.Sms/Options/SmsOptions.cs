namespace AGooday.AgPay.Notice.Sms
{
    public class SmsOptions
    {
        public const string SectionName = "Sms";

        /// <summary>
        /// 同一消息发送间隔
        /// </summary>
        public int? IntervalSeconds { get; set; }

        /// <summary>
        /// 使用短信服务类型：aliyunSms
        /// </summary>
        public string SmsUseType { get; set; }

        public AliyunSmsOptions AliyunSms { get; set; }
    }

    public class AliyunSmsOptions
    {
        public const string SectionName = "AliyunSms";

        public string Endpoint { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string AccountOpenTemplateId { get; set; }
        public string ForgetPwdTemplateId { get; set; }
        public string LoginMchTemplateId { get; set; }
        public string MbrTelBindTemplateId { get; set; }
        public string RegisterMchTemplateId { get; set; }
        public string SignName { get; set; }
    }
}
