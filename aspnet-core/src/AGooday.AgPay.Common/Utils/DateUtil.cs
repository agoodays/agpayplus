namespace AGooday.AgPay.Common.Utils
{
    public static class DateUtil
    {
        public const string TODAY = "today";
        public const string YESTERDAY = "yesterday";
        public const string NEAR2NOW = "near2now";
        public const string CUSTOM_DATE_TIME = "customDateTime";

        public static void GetQueryDateRange(string queryDateRange, out string createdStart, out string createdEnd)
        {
            queryDateRange ??= string.Empty;
            createdStart = null; createdEnd = null;
            if (queryDateRange.Equals(TODAY))
            {
                createdStart = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.Equals(YESTERDAY))
            {
                createdStart = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.StartsWith(NEAR2NOW))
            {
                int day = Convert.ToInt32(queryDateRange.Split("_")[1]);
                createdStart = DateTime.Today.AddDays(-(day - 1)).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = DateTime.Today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.StartsWith(CUSTOM_DATE_TIME))
            {
                createdStart = Convert.ToDateTime(queryDateRange.Split("_")[1]).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = Convert.ToDateTime(queryDateRange.Split("_")[2]).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
