namespace AGooday.AgPay.Agent.Api.Middlewares
{
    /// <summary>
    /// 设置 NDC 值
    /// </summary>
    public class NdcMiddleware
    {
        private readonly RequestDelegate _next;

        public NdcMiddleware(RequestDelegate next)
        {
            ArgumentNullException.ThrowIfNull(next);

            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ndcValue = Guid.NewGuid().ToString("N");

            // 设置 NDC 值
            // log4net.NDC.Push(ndcValue);
            // 将 NDC 值推入 LogicalThreadContext.Stacks
            // 使用 log4net 的 LogicalThreadContext.Stacks 属性，它提供了线程安全的堆栈管理，并确保每个线程都有独立的 NDC 堆栈。
            log4net.LogicalThreadContext.Stacks["NDC"].Push(ndcValue);

            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                // 清除 NDC 值 log4net.NDC 上下文
                // log4net.NDC.Pop(); 
                log4net.LogicalThreadContext.Stacks["NDC"].Pop();
            }
        }
    }
    public static class NdcMiddlewareExtensions
    {
        public static IApplicationBuilder UseNdc(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<NdcMiddleware>();
        }
    }
}
