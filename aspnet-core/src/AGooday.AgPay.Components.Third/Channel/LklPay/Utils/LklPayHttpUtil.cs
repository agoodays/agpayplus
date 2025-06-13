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
            // 参数校验
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL 不能为空", nameof(url));
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("appId 不能为空", nameof(appId));
            if (string.IsNullOrWhiteSpace(serialNo))
                throw new ArgumentException("serialNo 不能为空", nameof(serialNo));
            if (privateCert == null)
                throw new ArgumentNullException(nameof(privateCert));
            if (reqParams == null)
                throw new ArgumentNullException(nameof(reqParams));

            using (var client = new AgHttpClient(DEFAULT_TIMEOUT, DEFAULT_CHARSET))
            {
                // 生成紧凑且排序后的 JSON 字符串用于签名
                var sortedReqParams = new JObject(reqParams.Properties().OrderBy(p => p.Name));
                var contentForSign = sortedReqParams.ToString(Formatting.None);
                var authorization = LklPaySignUtil.GetAuthorizationHeader(appId, serialNo, contentForSign, privateCert);

                var request = new AgHttpClient.Request()
                {
                    Url = url,
                    Method = HttpMethod.Post.Method,
                    Content = JsonConvert.SerializeObject(sortedReqParams), // 使用排序后的参数
                    ContentType = MediaTypeNames.Application.Json,
                    Headers = new Dictionary<string, string> { { "Authorization", authorization } }
                };

                try
                {
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    // 始终返回内容和 headers，调用方根据 IsSuccessStatusCode 判断
                    return (response.Content, response.Headers);
                }
                catch (Exception e)
                {
                    // 记录详细日志（可选）
                    LogUtil<LklPayHttpUtil>.Error($"请求失败: {url}，{reqParams}", e);
                    throw ChannelException.SysError($"支付通道请求异常：{e.Message}");
                }
            }
        }
    }
}
