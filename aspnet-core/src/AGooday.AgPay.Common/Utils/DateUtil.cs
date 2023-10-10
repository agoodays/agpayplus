namespace AGooday.AgPay.Common.Utils
{
    public static class DateUtil
    {
        public static void GetQueryDateRange(string queryDateRange, out string createdStart, out string createdEnd)
        {
            createdStart = null; createdEnd = null;
            queryDateRange = queryDateRange ?? string.Empty;
            if (queryDateRange.Equals("today"))
            {
                createdStart = DateTime.Today.ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            if (queryDateRange.Equals("yesterday"))
            {
                createdStart = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.ToString("yyyy-MM-dd");
            }
            if (queryDateRange.Contains("near2now"))
            {
                int day = Convert.ToInt32(queryDateRange.Split("_")[1]);
                createdStart = DateTime.Today.AddDays(-(day - 1)).ToString("yyyy-MM-dd");
                createdEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
            if (queryDateRange.Contains("customDateTime"))
            {
                createdStart = Convert.ToDateTime(queryDateRange.Split("_")[1]).ToString("yyyy-MM-dd HH:mm:dd");
                createdEnd = Convert.ToDateTime(queryDateRange.Split("_")[2]).AddDays(1).ToString("yyyy-MM-dd HH:mm:dd");
            }
        }
    }
}
