using AGooday.AgPay.Notice.Core;
using AGooday.AgPay.Notice.Email;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmailExtension
    {
        public static NoticeOptions UseEmail(
            this NoticeOptions options,
            Action<EmailOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            options.RegisterExtension(new EmailOptionsExtension(configure));
            return options;
        }
    }
}