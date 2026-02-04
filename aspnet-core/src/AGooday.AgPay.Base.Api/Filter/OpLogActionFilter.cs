using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.OpLog;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AGooday.AgPay.Base.Api.Filter
{
    /// <summary>
    /// 操作日志过滤器
    /// </summary>
    public class OpLogActionFilter : IAsyncActionFilter
    {
        private readonly IOpLogHandler _logHandler;

        public OpLogActionFilter(IOpLogHandler logHandler)
        {
            _logHandler = logHandler;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodLogAttribute)))
            //if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(NoLogAttribute))
            //    && !context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodLogAttribute)))
            {
                return next();
            }

            return _logHandler.LogAsync(context, next);
        }
    }
}
