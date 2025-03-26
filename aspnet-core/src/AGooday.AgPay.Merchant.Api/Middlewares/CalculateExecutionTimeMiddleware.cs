﻿using System.Diagnostics;

namespace AGooday.AgPay.Merchant.Api.Middlewares
{
    /// <summary>
    /// 计算执行时间
    /// </summary>
    public class CalculateExecutionTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        Stopwatch stopwatch;
        public CalculateExecutionTimeMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(next);

            ArgumentNullException.ThrowIfNull(loggerFactory);

            _next = next;
            _logger = loggerFactory.CreateLogger<CalculateExecutionTimeMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //context.TraceIdentifier = Guid.NewGuid().ToString("N");
            stopwatch = new Stopwatch();
            stopwatch.Start();

            await _next.Invoke(context);

            stopwatch.Stop();
            _logger.LogInformation("[{TraceIdentifier}] RequestMethod: {RequestMethod}, RequestPath: {RequestPath}, ElapsedMilliseconds: {ElapsedMilliseconds} ms, Response StatusCode: {ResponseStatusCode}",
                context.TraceIdentifier, context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
            //_logger.LogInformation($"[{context.TraceIdentifier}] RequestMethod: {context.Request.Method}, RequestPath: {context.Request.Path}, ElapsedMilliseconds: {stopwatch.ElapsedMilliseconds} ms, Response StatusCode: {context.Response.StatusCode}");
        }
    }
    public static class CalculateExecutionTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseCalculateExecutionTime(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<CalculateExecutionTimeMiddleware>();
        }
    }
}
