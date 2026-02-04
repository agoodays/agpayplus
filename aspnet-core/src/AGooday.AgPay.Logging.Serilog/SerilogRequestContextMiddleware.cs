using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace AGooday.AgPay.Logging.Serilog
{
    public class SerilogRequestContextMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // 可选：自定义格式，如带前缀
            var crequestId = "REQ-" + Guid.NewGuid().ToString("N")[..16];
            //var requestId = context.TraceIdentifier;
            using (LogContext.PushProperty("CRequestId", crequestId))
            //using (LogContext.PushProperty("RequestId", requestId))
            using (LogContext.PushProperty("UserId", context.User.FindFirstValue("sui") ?? "anonymous"))
            {
                await next(context);
            }
        }
    }
    public static class SerilogRequestContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseAgSerilogRequestContext(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<SerilogRequestContextMiddleware>();
        }
    }
}
