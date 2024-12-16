using System.Net.Mime;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.LklPay.Utils
{
    public class LklPayHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static async Task<(string result, Dictionary<string, string> headers)> DoPostJsonAsync(string url, string appId, string serialNo, string privateCert, JObject reqParams)
        {
            Dictionary<string, string> headers = null;
            var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET);
            var authorization = LklPaySignUtil.GetAuthorizationHeader(appId, serialNo, reqParams.ToString(), privateCert);
            var request = new AgHttpClient.Request()
            {
                Url = url,
                Method = HttpMethod.Post.Method,
                Content = JsonConvert.SerializeObject(reqParams),
                ContentType = MediaTypeNames.Application.Json,
                Headers = new Dictionary<string, string> { { "Authorization", authorization } }
            };
            try
            {
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content;
                    headers = response.Headers;
                    return (result, headers);
                }
                return (null, headers);
            }
            catch (Exception e)
            {
                throw ChannelException.SysError(e.Message);
            }
        }
    }
}
