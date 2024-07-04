using Microsoft.AspNetCore.Mvc.Filters;

namespace AGooday.AgPay.Merchant.Api.OpLog
{
    /// <summary>
    /// 操作日志处理接口
    /// </summary>
    public interface IOpLogHandler
    {
        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task LogAsync(ActionExecutingContext context, ActionExecutionDelegate next);
    }
}
