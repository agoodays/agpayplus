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
            foreach (var header in request.Headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = new Response();
            try
            {
                HttpResponseMessage httpResponse = null;
                switch (request.Method)
                {
                    case "POST":
                        httpResponse = client.PostAsync(request.Url, new StringContent(request.Content, Encoding.UTF8, request.ContentType)).Result;
                        break;
                    case "GET":
                        httpResponse = client.GetAsync(request.Url).Result;
                        break;
                    case "PUT":
                        httpResponse = client.PutAsync(request.Url, new StringContent(request.Content, Encoding.UTF8, request.ContentType)).Result;
                        break;
                    case "DELETE":
                        httpResponse = client.DeleteAsync(request.Url).Result;
                        break;
                }
                response.StatusCode = httpResponse.StatusCode;
                response.Headers = httpResponse.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
                response.Content = httpResponse.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                // 日志和异常处理
                LogUtil<AgHttpClient>.Error("Http请求客户端异常", e);
            }
            return response;
        }

        public async Task<Response> SendAsync(Request request)
        {
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(timeout);
            foreach (var header in request.Headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = new Response();
            try
            {
                HttpResponseMessage httpResponse = null;
                switch (request.Method)
                {
                    case "POST":
                        httpResponse = await client.PostAsync(request.Url, new StringContent(request.Content, encoding, request.ContentType));
                        break;
                    case "GET":
                        httpResponse = await client.GetAsync(request.Url);
                        break;
                    case "PUT":
                        httpResponse = await client.PutAsync(request.Url, new StringContent(request.Content, encoding, request.ContentType));
                        break;
                    case "DELETE":
                        httpResponse = await client.DeleteAsync(request.Url);
                        break;
                }
                response.StatusCode = httpResponse.StatusCode;
                response.Headers = httpResponse.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
                response.Content = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                // 日志和异常处理
                LogUtil<AgHttpClient>.Error("Http请求客户端异常", e);
            }
            return response;
        }

        public class Response
        {
            public HttpStatusCode StatusCode { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public string Content { get; set; }
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
