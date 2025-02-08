using System.Net;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Agent.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;  // 用来处理上下文请求  
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); //要么在中间件中处理，要么被传递到下一个中间件中去
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex); // 捕获异常了 在HandleExceptionAsync中处理
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //context.Response.ContentType = MediaTypeNames.Application.Json;// 返回json 类型
            context.Response.ContentType = "application/json; charset=utf-8"; // 明确指定编码
            var response = context.Response;

            ApiRes errorResponse = ApiRes.Fail(ApiCode.SYSTEM_ERROR, exception.Message);
            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        errorResponse.Msg = ex.Message;
                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Msg = ex.Message;
                    break;
                //case KeyNotFoundException ex:
                //    //response.StatusCode = (int)HttpStatusCode.NotFound;
                //    //errorResponse.Msg = ex.Message;
                //    break;
                case UnauthorizeException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Code = ApiCode.SUCCESS.GetCode();
                    errorResponse.Msg = "登录失效";
                    break;
                case BizException ex:
                    errorResponse = ex.ApiRes;// 自定义的异常错误信息类型
                    //response.StatusCode = (int)HttpStatusCode.OK;
                    //errorResponse.Msg = ex.Message;
                    break;
                default:
                    //response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    //errorResponse.Msg = "Internal Server errors. Check Logs!";
                    break;
            }
            _logger.LogError(exception, $"[{context.TraceIdentifier}] {exception.Message}");
            var result = JsonConvert.SerializeObject(errorResponse);
            await response.WriteAsync(result);
        }
    }

    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}