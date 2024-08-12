﻿namespace AGooday.AgPay.Common.Utils
{
    public static class DateUtil
    {
        /// <summary>
        /// 今天
        /// </summary>
        public const string TODAY = "today";
        /// <summary>
        /// 昨天
        /// </summary>
        public const string YESTERDAY = "yesterday";
        /// <summary>
        /// 当月
        /// </summary>
        public const string CURR_MONTH = "currMonth";
        /// <summary>
        /// 上月
        /// </summary>
        public const string PREV_MONTH = "prevMonth";
        /// <summary>
        /// 近xx天，到今天
        /// </summary>
        public const string NEAR2NOW = "near2now";
        /// <summary>
        /// 自定义日期时间格式
        /// </summary>
        public const string CUSTOM_DATE_TIME = "customDateTime";

        public static void GetQueryDateRange(string queryDateRange, out string createdStart, out string createdEnd)
        {
            queryDateRange ??= string.Empty;
            createdStart = null; createdEnd = null;
            var today = DateTime.Today;
            if (queryDateRange.Equals(TODAY))
            {
                createdStart = today.ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.Equals(YESTERDAY))
            {
                createdStart = today.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = today.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.Equals(CURR_MONTH))
            {
                createdStart = today.AddDays(1 - today.Day).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.Equals(PREV_MONTH))
            {
                var firstDayOfMonth = today.AddDays(1 - today.Day);
                createdStart = firstDayOfMonth.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = firstDayOfMonth.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.StartsWith(NEAR2NOW))
            {
                int day = Convert.ToInt32(queryDateRange.Split("_")[1]);
                createdStart = today.AddDays(-(day - 1)).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = today.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryDateRange.StartsWith(CUSTOM_DATE_TIME))
            {
                createdStart = Convert.ToDateTime(queryDateRange.Split("_")[1]).ToString("yyyy-MM-dd HH:mm:ss");
                createdEnd = Convert.ToDateTime(queryDateRange.Split("_")[2]).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
