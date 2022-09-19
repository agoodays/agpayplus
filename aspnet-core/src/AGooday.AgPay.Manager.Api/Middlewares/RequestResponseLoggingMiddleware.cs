using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace AGooday.AgPay.Manager.Api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        private SortedDictionary<string, object> _data;
        private Stopwatch _stopwatch;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task Invoke(HttpContext context)
        {
            _stopwatch.Restart();
            _data = new SortedDictionary<string, object>();

            HttpRequest request = context.Request;
            _data.Add("request.url", request.Path.ToString());
            _data.Add("request.headers", request.Headers.ToDictionary(x => x.Key, v => string.Join(";", v.Value.ToList())));
            _data.Add("request.method", request.Method);
            _data.Add("request.executeStartTime", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            // 获取请求body内容
            if (request.Method.ToLower().Equals("post"))
            {
                // 启用倒带功能，就可以让 Request.Body 可以再次读取
                request.EnableBuffering();

                Stream stream = request.Body;
                byte[] buffer = new byte[request.ContentLength.Value];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                _data.Add("request.body", Encoding.UTF8.GetString(buffer));

                request.Body.Position = 0;
            }
            else if (request.Method.ToLower().Equals("get"))
            {
                _data.Add("request.body", request.QueryString.Value);
            }

            // 获取Response.Body内容
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                _data.Add("response.body", await GetResponse(context.Response));
                _data.Add("response.executeEndTime", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                await responseBody.CopyToAsync(originalBodyStream);
            }

            // 响应完成记录时间和存入日志
            context.Response.OnCompleted(() =>
            {
                _stopwatch.Stop();
                _data.Add("elaspedTime", _stopwatch.ElapsedMilliseconds + "ms");
                var json = JsonConvert.SerializeObject(_data);
                _logger.LogDebug(json);
                return Task.CompletedTask;
            });

        }

        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }

    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
