using System.Diagnostics;

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
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this._next = next;
            _logger = loggerFactory.CreateLogger<CalculateExecutionTimeMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //context.TraceIdentifier = Guid.NewGuid().ToString("N");
            stopwatch = new Stopwatch();
            stopwatch.Start();

            await _next.Invoke(context);

            stopwatch.Stop();
            _logger.LogInformation($"[{context.TraceIdentifier}], RequestMethod: {context.Request.Method}, RequestPath: {context.Request.Path}, ElapsedMilliseconds: {stopwatch.ElapsedMilliseconds}, Response StatusCode: {context.Response.StatusCode}");
        }
    }
    public static class CalculateExecutionTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseCalculateExecutionTime(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CalculateExecutionTimeMiddleware>();
        }
    }
}
