namespace AGooday.AgPay.Notice.Core
{
    public class DateTimeHelper
    {
        public static long GetTimestamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
