using AGooday.AgPay.Application.Interfaces;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 退款订单过期定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class RefundOrderExpiredJob : IJob
    {
        private readonly ILogger<RefundOrderExpiredJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RefundOrderExpiredJob(ILogger<RefundOrderExpiredJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var refundOrderService = scope.ServiceProvider.GetService<IRefundOrderService>();
                int updateCount = await refundOrderService.UpdateOrderExpiredAsync();
                _logger.LogInformation($"处理退款订单超时{updateCount}条.");
            }
        }
    }
}
