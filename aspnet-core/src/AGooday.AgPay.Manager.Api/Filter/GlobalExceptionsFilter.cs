using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AGooday.AgPay.Manager.Api.Filter
{
    /// <summary>
    /// 全局异常错误日志
    /// </summary>
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionsFilter> _logger;

        public GlobalExceptionsFilter(IWebHostEnvironment env, ILogger<GlobalExceptionsFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            ApiRes errorResponse = ApiRes.Fail(ApiCode.SYSTEM_ERROR, context.Exception.Message);
            var errorAudit = "Unable to resolve service for";//特殊错误信息
            if (!string.IsNullOrEmpty(errorResponse.Msg) && errorResponse.Msg.Contains(errorAudit))
            {
                errorResponse.Msg = errorResponse.Msg.Replace(errorAudit, $"（若新添加服务，需要重新编译项目）{errorAudit}");
            }
            if (_env.IsDevelopment())
            {
                errorResponse.Msg = context.Exception.StackTrace;//堆栈信息
            }
            context.Result = new InternalServerErrorObjectResult(errorResponse);

            //输出错误日志信息
            _logger.LogError(context.Exception, "\r\n【自定义错误】：{Msg} \r\n【异常类型】：{TypeName} \r\n【异常信息】：{ExceptionMessage} \r\n【堆栈调用】：{StackTrace}",
                errorResponse.Msg, context.Exception.GetType().Name, context.Exception.Message, context.Exception.StackTrace);
            //_logger.LogError(context.Exception, $"\r\n【自定义错误】：{errorResponse.Msg} \r\n【异常类型】：{context.Exception.GetType().Name} \r\n【异常信息】：{context.Exception.Message} \r\n【堆栈调用】：{context.Exception.StackTrace}");
        }
    }

    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
