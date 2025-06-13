using System.Net.Mime;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay.Utils
{
    public class UmsPayHttpUtil
    {
        private static readonly string DEFAULT_CHARSET = "UTF-8";
        private static readonly int DEFAULT_TIMEOUT = 60; // 60 秒超时

        public static async Task<string> DoPostJsonAsync(string url, string appid, string appkey, JObject reqParams)
        {
            // 参数校验
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL 不能为空", nameof(url));
            if (string.IsNullOrWhiteSpace(appid))
                throw new ArgumentException("appid 不能为空", nameof(appid));
            if (string.IsNullOrWhiteSpace(appkey))
                throw new ArgumentException("appkey 不能为空", nameof(appkey));
            if (reqParams == null)
                throw new ArgumentNullException(nameof(reqParams));

            using (var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET))
            {
                // 生成紧凑 JSON（移除缩进）
                var body = JsonConvert.SerializeObject(reqParams, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                // 生成时间戳和随机数
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string nonce = Guid.NewGuid().ToString("N"); // 简化随机数生成

                // 生成签名
                var authorization = UmsPaySignUtil.GetAuthorization(appid, appkey, timestamp, nonce, body);

                var request = new AgHttpClient.Request()
                {
                    Url = url,
                    Method = HttpMethod.Post.Method,
                    Content = body,
                    ContentType = MediaTypeNames.Application.Json,
                    Headers = new Dictionary<string, string> { { "Authorization", authorization } }
                };

                try
                {
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    return response.Content; // 始终返回内容
                }
                catch (Exception e)
                {
                    // 记录脱敏日志（示例）
                    LogUtil<UmsPayHttpUtil>.Error($"请求失败: {url}，{reqParams}", e);
                    throw ChannelException.SysError($"支付通道请求异常：{e.Message}");
                }
            }
        }
    }
}
