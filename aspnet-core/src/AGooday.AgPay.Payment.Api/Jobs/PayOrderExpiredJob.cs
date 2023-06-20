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
        private readonly ILogger<PayOrderExpiredJob> logger;
        private readonly IPayOrderService payOrderService;

        public PayOrderExpiredJob(ILogger<PayOrderExpiredJob> logger,
            IPayOrderService payOrderService)
        {
            this.logger = logger;
            this.payOrderService = payOrderService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                int updateCount = payOrderService.UpdateOrderExpired();
                logger.LogInformation($"处理订单超时{updateCount}条.");
            });
        }
    }
}
