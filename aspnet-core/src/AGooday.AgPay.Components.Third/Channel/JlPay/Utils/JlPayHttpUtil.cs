using System.Net.Mime;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.JlPay.Utils
{
    public class JlPayHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static async Task<string> DoPostJsonAsync(string url, JObject reqParams)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL 不能为空", nameof(url));
            if (reqParams == null)
                throw new ArgumentNullException(nameof(reqParams));

            // 使用 using 确保资源释放
            using (var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET))
            {
                var request = new AgHttpClient.Request()
                {
                    Url = url,
                    Method = HttpMethod.Post.Method,
                    Content = JsonConvert.SerializeObject(reqParams, Formatting.None), // 紧凑 JSON
                    ContentType = MediaTypeNames.Application.Json
                };

                try
                {
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content;
                        return result;
                    }
                    return null;
                }
                catch (Exception e)
                {
                    // 记录详细日志（可选）
                    LogUtil<JlPayHttpUtil>.Error($"请求失败: {url}，{reqParams}", e);
                    throw ChannelException.SysError($"支付通道请求异常：{e.Message}");
                }
            }
        }
    }
}
