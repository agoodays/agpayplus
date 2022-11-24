namespace AGooday.AgPay.AopSdk
{
    public class AgPay
    {
        public const string LIVE_API_BASE = "https://pay.agpay.com";
        public const string VERSION = "1.0";
        public const string DEFAULT_SIGN_TYPE = "MD5";
        public const string API_VERSION_NAME = "version";
        public const string API_SIGN_TYPE_NAME = "signType";
        public const string API_SIGN_NAME = "sign";
        public const string API_REQ_TIME_NAME = "reqTime";

        /// <summary>
        /// 默认时间格式
        /// </summary>
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Date默认时区
        /// </summary>
        public const string DATE_TIMEZONE = "GMT+8";

        public static string AcceptLanguage { get; set; } = "zh-CN";

        public static volatile string MchNo;

        public static volatile string AppId;

        /// <summary>
        /// 私钥
        /// </summary>
        public static volatile string ApiKey;

        /// <summary>
        /// API 地址
        /// </summary>
        public static string ApiBase { get; set; } = LIVE_API_BASE;

        //public static volatile string privateKey;
        //public static volatile string privateKeyPath;

        //public static bool DEBUG = false;

        public const int DEFAULT_CONNECT_TIMEOUT = 30 * 1000;
        public const int DEFAULT_READ_TIMEOUT = 80 * 1000;

        private static int ConnectTimeout = -1;

        private static int ReadTimeout = -1;

        /// <summary>
        /// 设置连接失败时的最大重试次数
        /// </summary>
        public static int MaxNetworkRetries { get; set; }= 1;

        /// <summary>
        /// 网络连接超时时间
        /// </summary>
        /// <returns></returns>
        public static int GetConnectTimeout()
        {
            if (ConnectTimeout == -1)
            {
                return DEFAULT_CONNECT_TIMEOUT;
            }
            return ConnectTimeout;
        }

        /// <summary>
        /// 设置网络连接超时时间 (毫秒)
        /// </summary>
        /// <param name="timeout"></param>
        public static void SetConnectTimeout(int timeout)
        {
            ConnectTimeout = timeout;
        }

        /// <summary>
        /// 数据读取超时时间
        /// </summary>
        /// <returns></returns>
        public static int GetReadTimeout()
        {
            if (ReadTimeout == -1)
            {
                return DEFAULT_READ_TIMEOUT;
            }
            return ReadTimeout;
        }

        /// <summary>
        /// 设置数据读取超时时间 (毫秒)
        /// 不同接口的耗时时间不一样，部分接口的耗时可能比较长。
        /// </summary>
        /// <param name="timeout"></param>
        public static void SetReadTimeout(int timeout)
        {
            ReadTimeout = timeout;
        }
    }
}