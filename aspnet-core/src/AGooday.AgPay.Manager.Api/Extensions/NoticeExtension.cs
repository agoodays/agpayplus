using AGooday.AgPay.Notice.Core;
using AGooday.AgPay.Notice.Email;

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
            });

            return services;
        }
    }
}
