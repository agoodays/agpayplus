using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Payment.Api.Channel.LesPay.Utils
{
    public class LesHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static string DoPost(string url, string reqParams)
        {
            var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET);
            var request = new AgHttpClient.Request()
            {
                Url = url,
                Method = "POST",
                Content = reqParams,
                ContentType = "application/x-www-form-urlencoded"
            };
            var response = client.Send(request);
            string result = response.Content;
            return result;
        }
    }
}
