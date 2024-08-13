namespace AGooday.AgPay.Application.Config
{
    public class DBSmsConfig
    {
        /// <summary>
        /// 短信使用厂商
        /// </summary>
        public string SmsProviderKey { get; set; }

        /// <summary>
        /// [吉日短信]短信配置
        /// </summary>
        public string AgpaydxSmsConfig { get; set; }

        /// <summary>
        /// 阿里云短信服务
        /// </summary>
        public string AliyundySmsConfig { get; set; }

        /// <summary>
        /// [模拟测试]短信配置
        /// </summary>
        public string MocktestSmsConfig { get; set; }
    }
}
