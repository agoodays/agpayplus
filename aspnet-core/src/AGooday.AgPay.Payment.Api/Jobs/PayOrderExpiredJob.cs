using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Cache.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 订单过期定时任务
    /// </summary>
    [DisallowConcurrentExecution] // 防止单实例内并发
    public class PayOrderExpiredJob : AbstractJob
    {
        private readonly IPayOrderService _payOrderService;

        public PayOrderExpiredJob(
            ILogger<PayOrderExpiredJob> logger,
            ICacheService cacheService,
            IPayOrderService payOrderService)
            : base(logger, cacheService)
        {
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 订单过期任务通常执行快，设置较短的锁超时时间
        /// </summary>
        protected override TimeSpan GetLockExpiry() => TimeSpan.FromMinutes(2);

        /// <summary>
        /// 最大执行时间 3 分钟
        /// </summary>
        protected override TimeSpan GetMaxExecutionTime() => TimeSpan.FromMinutes(3);

        /// <summary>
        /// Redis 不可用时不执行（保守策略）
        /// </summary>
        protected override bool AllowFallbackExecution() => false;

        public override async Task Execute(IJobExecutionContext context)
        {
            await ExecuteLockTakeAsync(context.JobDetail.Key.ToString(), async () =>
            {
                int updateCount = await _payOrderService.UpdateOrderExpiredAsync();

                _logger.LogInformation("任务 [{JobKey}] 处理订单超时 {UpdateCount} 条。",
                    context.JobDetail.Key, updateCount);
            });
        }

        protected override Task OnExecutionFailed(string jobKey, Exception exception)
        {
            _logger.LogError("⚠️ 订单过期任务执行失败，需要人工介入检查！");
            return Task.CompletedTask;
        }
    }
}
