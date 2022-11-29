using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 订单过期定时任务
    /// </summary>
    public class PayOrderExpiredJob : IJob
    {
        private readonly IPayOrderService payOrderService;

        public PayOrderExpiredJob(IPayOrderService payOrderService)
        {
            this.payOrderService = payOrderService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                int updateCount = payOrderService.UpdateOrderExpired();
                LogUtil<PayOrderExpiredJob>.Info($"处理订单超时{updateCount}条.");
            });
        }
    }
}
