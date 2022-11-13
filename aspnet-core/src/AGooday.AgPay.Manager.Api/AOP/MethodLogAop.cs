using Castle.DynamicProxy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace AGooday.AgPay.Manager.Api.AOP
{
    public class MethodLogAop : IInterceptor
    {
        private readonly ILogger<MethodLogAop> _logger;
        private readonly IHttpContextAccessor _accessor;

        public MethodLogAop(ILogger<MethodLogAop> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
        }
        /// <summary>
        /// 实例化IInterceptor唯一方法 
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            var name = $"{invocation.Method.DeclaringType}.{invocation.Method.Name}";
            var args = string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()));
            var watch = System.Diagnostics.Stopwatch.StartNew();
            invocation.Proceed(); //Intercepted method is executed here.
            watch.Stop();
            var executionTime = watch.ElapsedMilliseconds;
            _logger.LogInformation($"MethodLogAop: {name}{args}{invocation.ReturnValue}{executionTime} ms");
        }
    }
}
