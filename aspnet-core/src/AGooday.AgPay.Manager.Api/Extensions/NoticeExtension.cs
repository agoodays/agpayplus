using AGooday.AgPay.Notice.Core;
using AGooday.AgPay.Notice.Email;
using AGooday.AgPay.Notice.Sms;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    public static class NoticeExtension
    {
        public static IServiceCollection AddNotice(this IServiceCollection services, IConfiguration configuration, Action<NoticeOptions> configure = null, string sectionName = null)
        {
            sectionName ??= NoticeOptions.SectionName;

            var baseConfiguration = configuration.GetSection(sectionName);
            var noticeOptions = baseConfiguration.Get<NoticeOptions>();
            configure?.Invoke(noticeOptions);

            services.AddNotice(config =>
            {
                //同一消息发送间隔 默认10秒
                config.IntervalSeconds = noticeOptions.IntervalSeconds;

                var mailOptions = baseConfiguration.GetSection(EmailOptions.SectionName).Get<EmailOptions>();
                if (mailOptions != null)
                {
                    config.UseEmail(x =>
                    {
                        x.Password = mailOptions.Password;
                        x.Host = mailOptions.Host;
                        x.FromAddress = mailOptions.FromAddress;
                        x.FromName = mailOptions.FromName;
                        x.Port = mailOptions.Port;
                        x.ToAddress = mailOptions.ToAddress;
                    });
                }

                var smsConfiguration = baseConfiguration.GetSection(SmsOptions.SectionName);
                var smsOptions = smsConfiguration.Get<SmsOptions>();
                if (smsOptions != null)
                {
                    config.UseSms(x =>
                    {
                        x.IntervalSeconds = 0;
                        x.SmsUseType = smsOptions.SmsUseType;
                        x.AliyunSms = smsOptions.AliyunSms;
                        //x.AliyunSms = smsConfiguration.GetSection(AliyunSmsOptions.SectionName).Get<AliyunSmsOptions>();
                    });
                }
            });

            return services;
        }
    }
}
