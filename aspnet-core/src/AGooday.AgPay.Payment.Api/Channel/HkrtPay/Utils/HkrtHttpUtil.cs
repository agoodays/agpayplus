using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mime;

namespace AGooday.AgPay.Payment.Api.Channel.HkrtPay.Utils
{
    public class HkrtHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static string DoPost(string url, JObject reqParams)
        {
            var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET);
            var request = new AgHttpClient.Request()
            {
                Url = url,
                Method = HttpMethod.Post.Method,
                Content = JsonConvert.SerializeObject(reqParams),
                ContentType = MediaTypeNames.Application.FormUrlEncoded
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
