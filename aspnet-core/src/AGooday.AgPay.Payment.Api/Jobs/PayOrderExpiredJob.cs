using AGooday.AgPay.Application.Interfaces;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 订单过期定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PayOrderExpiredJob : IJob
    {
        private readonly ILogger<PayOrderExpiredJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PayOrderExpiredJob(ILogger<PayOrderExpiredJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var payOrderService = scope.ServiceProvider.GetService<IPayOrderService>();
                int updateCount = await payOrderService.UpdateOrderExpiredAsync();
                _logger.LogInformation($"处理订单超时{updateCount}条.");
            }
        }
    }
}
