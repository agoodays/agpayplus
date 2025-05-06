using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Cache.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 订单过期定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PayOrderExpiredJob : AbstractJob
    {
        public PayOrderExpiredJob(ILogger<PayOrderExpiredJob> logger,
            IServiceScopeFactory serviceScopeFactory,
            ICacheService cacheService)
            : base(logger, serviceScopeFactory, cacheService)
        {
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await ExecuteLockTakeAsync(context.JobDetail.Key.ToString(), async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var payOrderService = scope.ServiceProvider.GetService<IPayOrderService>();
                    int updateCount = await payOrderService.UpdateOrderExpiredAsync();

                    _logger.LogInformation("任务 [{JobKey}] 处理订单超时 {UpdateCount} 条。", context.JobDetail.Key, updateCount);
                    //_logger.LogInformation($"处理订单超时{updateCount}条.");
                }
            });
        }
    }
}
