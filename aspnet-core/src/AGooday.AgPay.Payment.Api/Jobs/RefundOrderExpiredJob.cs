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
        private readonly ILogger<RefundOrderExpiredJob> logger;
        private readonly IRefundOrderService refundOrderService;

        public RefundOrderExpiredJob(ILogger<RefundOrderExpiredJob> logger, IRefundOrderService refundOrderService)
        {
            this.logger = logger;
            this.refundOrderService = refundOrderService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                int updateCount = refundOrderService.UpdateOrderExpired();
                logger.LogInformation($"处理退款订单超时{updateCount}条.");
            });
        }
    }
}
