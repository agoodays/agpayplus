using System.Diagnostics;

namespace AGooday.AgPay.Manager.Api.Middlewares
{
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

        public async Task Invoke(HttpContext context)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            await _next.Invoke(context);

            stopwatch.Stop();
            _logger.LogInformation($@"接口{context.Request.Path}耗时{stopwatch.ElapsedMilliseconds}ms");
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
