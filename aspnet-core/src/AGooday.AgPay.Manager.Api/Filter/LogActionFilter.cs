using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Logs;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AGooday.AgPay.Manager.Api.Filter
{
    /// <summary>
    /// 操作日志过滤器
    /// </summary>
    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly ILogHandler _logHandler;

        public LogActionFilter(ILogHandler logHandler)
        {
            _logHandler = logHandler;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodRemarkAttribute)))
            //if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(NoLogAttribute))
            //    && !context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodRemarkAttribute)))
            {
                return next();
            }

            return _logHandler.LogAsync(context, next);
        }
    }
}
