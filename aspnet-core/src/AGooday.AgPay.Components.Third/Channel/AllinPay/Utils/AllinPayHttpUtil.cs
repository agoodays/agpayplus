using System.Net.Mime;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay.Utils
{
    public class AllinPayHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static async Task<string> DoPostJsonAsync(string url, JObject reqParams)
        {
            // 参数校验
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL 不能为空", nameof(url));
            if (reqParams == null)
                throw new ArgumentNullException(nameof(reqParams));

            // 使用 using 确保 AgHttpClient 释放。
            using (var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET))
            {
                var request = new AgHttpClient.Request()
                {
                    Url = url,
                    Method = HttpMethod.Post.Method,
                    Content = JsonConvert.SerializeObject(reqParams),
                    ContentType = MediaTypeNames.Application.Json
                };

                try
                {
                    // 添加 ConfigureAwait(false) 避免不必要的上下文捕获。
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    // 直接返回 response.Content，由调用方根据状态码处理。
                    return response.Content; // 始终返回内容
                }
                catch (Exception e)
                {
                    // 记录详细日志（可选）
                    LogUtil<AllinPayHttpUtil>.Error($"请求失败: {url}，{reqParams}", e);
                    throw ChannelException.SysError($"请求失败: {e.Message}");
                }
            }
        }
    }
}
