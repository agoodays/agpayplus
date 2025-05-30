using System.Net;
using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    public class AgHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Encoding _encoding;

        public AgHttpClient(int timeout = 60, string charset = "UTF-8")
        {
            _encoding = Encoding.GetEncoding(charset);
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };
        }

        public async Task<Response> SendAsync(Request request)
        {
            var response = new Response();
            HttpResponseMessage httpResponse = null;
            try
            {
                var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), request.Url);

                // 添加请求头
                if (request.Headers != null)
                {
                    foreach (var header in request.Headers)
                    {
                        httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                // 设置请求内容
                if (request.Method == "POST" || request.Method == "PUT")
                {
                    httpRequest.Content = new StringContent(request.Content, _encoding, request.ContentType);
                }

                // 发送请求
                httpResponse = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);

                // 解析响应
                response.StatusCode = httpResponse.StatusCode;
                response.Headers = httpResponse.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
                response.Content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                // 处理特定状态码信息
                if (httpResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    response.Content = "请求超时";
                }
                else if (httpResponse.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    response.Content = "网关超时";
                }
            }
            catch (HttpRequestException ex)
            {
                LogUtil<AgHttpClient>.Error("HTTP请求异常", ex);
                throw new ApplicationException($"请求失败，URL: {request.Url}", ex);
            }
            catch (Exception ex)
            {
                LogUtil<AgHttpClient>.Error("未知异常", ex);
                throw;
            }
            finally
            {
                httpResponse?.Dispose();
            }

            return response;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public class Response
        {
            public HttpStatusCode StatusCode { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public string Content { get; set; }

            public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;
        }

        public class Request
        {
            public string Url { get; set; }
            public string Method { get; set; }
            public string Content { get; set; }
            public string ContentType { get; set; }
            public Dictionary<string, string> Headers { get; set; }
        }
    }
}