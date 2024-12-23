using System.Net;
using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    public class AgHttpClient
    {
        private readonly HttpClientHandler handler;
        private readonly int timeout;
        private readonly string charset;
        private readonly Encoding encoding;

        public AgHttpClient(int timeout = 60, string charset = "UTF-8")
        {
            handler = new HttpClientHandler();
            this.timeout = timeout;
            this.charset = charset;
            this.encoding = Encoding.GetEncoding(charset);
        }

        public Response Send(Request request)
        {
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(timeout);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = new Response();
            try
            {
                StringContent content = null;
                HttpResponseMessage httpResponse = null;
                switch (request.Method)
                {
                    case "POST":
                        content = new StringContent(request.Content, encoding, request.ContentType);
                        httpResponse = client.PostAsync(request.Url, content).Result;
                        break;
                    case "GET":
                        httpResponse = client.GetAsync(request.Url).Result;
                        break;
                    case "PUT":
                        content = new StringContent(request.Content, encoding, request.ContentType);
                        httpResponse = client.PutAsync(request.Url, content).Result;
                        break;
                    case "DELETE":
                        httpResponse = client.DeleteAsync(request.Url).Result;
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Content = "Invalid request method.";
                        return response;
                }
                httpResponse.EnsureSuccessStatusCode();
                if (httpResponse.IsSuccessStatusCode)
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
                    response.Content = httpResponse.Content.ReadAsStringAsync().Result;
                }
                else if (httpResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    response.StatusCode = HttpStatusCode.GatewayTimeout;
                    response.Content = "The request has timed out.";
                }
                else if (httpResponse.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    response.StatusCode = HttpStatusCode.GatewayTimeout;
                    response.Content = "The gateway has timed out.";
                }
                else
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Content = httpResponse.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException e)
            {
                // 日志和异常处理
                LogUtil<AgHttpClient>.Error("Http请求客户端异常", e);
                throw;
            }
            catch (Exception e)
            {
                // 日志和异常处理
                LogUtil<AgHttpClient>.Error("Http客户端异常", e);
                throw;
            }
            return response;
        }

        public async Task<Response> SendAsync(Request request)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = new Response();
            try
            {
                StringContent content = null;
                HttpResponseMessage httpResponse = null;
                switch (request.Method)
                {
                    case "POST":
                        content = new StringContent(request.Content, encoding, request.ContentType);
                        httpResponse = await client.PostAsync(request.Url, content);
                        break;
                    case "GET":
                        httpResponse = await client.GetAsync(request.Url);
                        break;
                    case "PUT":
                        content = new StringContent(request.Content, encoding, request.ContentType);
                        httpResponse = await client.PutAsync(request.Url, content);
                        break;
                    case "DELETE":
                        httpResponse = await client.DeleteAsync(request.Url);
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Content = "Invalid request method.";
                        return response;
                }
                httpResponse.EnsureSuccessStatusCode();
                if (httpResponse.IsSuccessStatusCode)
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
                    response.Content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else if (httpResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    response.StatusCode = HttpStatusCode.RequestTimeout;
                    response.Content = "The request has timed out.";
                }
                else if (httpResponse.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    response.StatusCode = HttpStatusCode.GatewayTimeout;
                    response.Content = "The gateway has timed out.";
                }
                else
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (HttpRequestException e)
            {
                // 日志和异常处理
                LogUtil<AgHttpClient>.Error("Http请求客户端异常", e);
                throw;
            }
            catch (Exception e)
            {
                // 日志和异常处理
                LogUtil<AgHttpClient>.Error("Http客户端异常", e);
                throw;
            }
            return response;
        }

        public class Response
        {
            public HttpStatusCode StatusCode { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public string Content { get; set; }

            public bool IsSuccessStatusCode
            {
                get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
            }
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
