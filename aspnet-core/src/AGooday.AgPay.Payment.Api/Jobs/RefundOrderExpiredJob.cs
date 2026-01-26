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
        private readonly IRefundOrderService _refundOrderService;

        public RefundOrderExpiredJob(
            ILogger<RefundOrderExpiredJob> logger,
            ICacheService cacheService,
            IRefundOrderService refundOrderService)
            : base(logger, cacheService)
        {
            _refundOrderService = refundOrderService;
        }

        protected override TimeSpan GetLockExpiry() => TimeSpan.FromMinutes(2);
        protected override TimeSpan GetMaxExecutionTime() => TimeSpan.FromMinutes(3);
        protected override bool AllowFallbackExecution() => false;

        public override async Task Execute(IJobExecutionContext context)
        {
            await ExecuteLockTakeAsync(context.JobDetail.Key.ToString(), async () =>
            {
                int updateCount = await _refundOrderService.UpdateOrderExpiredAsync();
                _logger.LogInformation("任务 [{JobKey}] 处理退款订单超时 {UpdateCount} 条。",
                    context.JobDetail.Key, updateCount);
            });
        }
    }
}
