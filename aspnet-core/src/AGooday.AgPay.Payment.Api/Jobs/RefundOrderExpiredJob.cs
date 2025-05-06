using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Cache.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 退款订单过期定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class RefundOrderExpiredJob : AbstractJob
    {
        public RefundOrderExpiredJob(ILogger<RefundOrderExpiredJob> logger,
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
                    var refundOrderService = scope.ServiceProvider.GetService<IRefundOrderService>();
                    int updateCount = await refundOrderService.UpdateOrderExpiredAsync();
                    _logger.LogInformation("任务 [{JobKey}] 处理退款订单超时{updateCount}条.", context.JobDetail.Key, updateCount);
                    //_logger.LogInformation($"处理退款订单超时{updateCount}条.");
                }
            });
        }
    }
}
