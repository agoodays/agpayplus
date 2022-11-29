using AGooday.AgPay.Payment.Api.Utils;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    public class QuartzHostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var trigger = TriggerBuilder.Create()
                .WithDescription("订单过期定时任务")
                .WithIdentity("payment.api.payorder.trigger")
                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0/1 * * * ?").WithMisfireHandlingInstructionDoNothing())// 每分钟执行一次
                                                                                                                          //.WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                .Build();
            JobKey jobKey = new JobKey("payment.api", "payorder");
            QuartzUtil.Add(typeof(PayOrderExpiredJob), jobKey, trigger).Wait();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
