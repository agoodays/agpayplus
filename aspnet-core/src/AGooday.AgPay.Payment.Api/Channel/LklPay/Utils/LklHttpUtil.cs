using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mime;

namespace AGooday.AgPay.Payment.Api.Channel.LklPay.Utils
{
    public class LklHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static string DoPostJson(string url, string appId, string serialNo, string privateCert, JObject reqParams, out Dictionary<string, string> headers)
        {
            headers = null;
            var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET);
            var authorization = LklSignUtil.GetAuthorizationHeader(appId, serialNo, reqParams.ToString(), privateCert);
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
                var response = client.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content;
                    headers = response.Headers;
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
