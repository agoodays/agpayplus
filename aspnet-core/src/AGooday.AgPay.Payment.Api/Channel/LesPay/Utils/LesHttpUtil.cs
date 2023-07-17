using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Exceptions;

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
            try
            {
                var response = client.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content;
                    return result;
                }
                return null;
            }
            catch (Exception e)
            {
                throw ChannelException.SysError(e.Message);
            }
        }
    }
}
