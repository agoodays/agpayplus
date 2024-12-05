﻿using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 过滤，只有接口
            var excludedPaths = new string[]
            {
                "/ossFiles",
                "/localOssFiles",
                "/auth/vercode",
                "/qrc/view",
                "/qrc/shell/view",
                "/qrc/shell/imgview",
                "/qrc/shell/nostyle.png",
                "/qrc/shell/stylea.png",
                "/qrc/shell/styleb.png",
                "/export/"
            };
            var requestPath = context.Request.Path.Value;
            var isApiPath = requestPath.StartsWith("/api");
            var isExcludedPath = excludedPaths.Any(path => requestPath.Contains(path, StringComparison.OrdinalIgnoreCase));
            if (isApiPath && !isExcludedPath)
            {
                context.Request.EnableBuffering();
                Stream originalBody = context.Response.Body;

                try
                {
                    // 存储请求数据
                    await RequestDataLog(context);

                    using var ms = new MemoryStream();
                    context.Response.Body = ms;

                    await _next(context);

                    // 存储响应数据
                    ResponseDataLog(context, ms);

                    ms.Position = 0;
                    await ms.CopyToAsync(originalBody);
                }
                catch (Exception ex)
                {
                    // 记录异常
                    _logger.LogError(ex, $"[{context.TraceIdentifier}] {ex.Message}");
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task RequestDataLog(HttpContext context)
        {
            var request = context.Request;
            var sr = new StreamReader(request.Body);
            var content = new
            {
                url = request.Path.Value,
                headers = request.Headers.ToDictionary(x => x.Key, v => string.Join(";", v.Value.ToList())),
                method = request.Method,
                query = request.QueryString,
                body = await sr.ReadToEndAsync(),
            };

            _logger.LogInformation($"[{context.TraceIdentifier}] RequestData:{JsonConvert.SerializeObject(content)}");

            request.Body.Position = 0;
        }

        private void ResponseDataLog(HttpContext context, MemoryStream ms)
        {
            ms.Position = 0;
            var responseBody = new StreamReader(ms).ReadToEnd();

            // 去除 Html
            //var reg = "<[^>]+>";
            //var isHtml = Regex.IsMatch(responseBody, reg);

            if (!string.IsNullOrEmpty(responseBody))
            {
                _logger.LogInformation($"[{context.TraceIdentifier}] ResponseData:{responseBody}");
            }
        }
    }

    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
