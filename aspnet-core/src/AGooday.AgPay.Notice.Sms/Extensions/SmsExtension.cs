using AGooday.AgPay.Notice.Core;
using AGooday.AgPay.Notice.Sms;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmsExtension
    {
        public static NoticeOptions UseSms(
            this NoticeOptions options,
            Action<SmsOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            options.RegisterExtension(new SmsOptionsExtension(configure));
            return options;
        }
    }
}