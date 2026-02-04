using Quartz;
using Quartz.Spi;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// Quartz 调度器托管服务
    /// </summary>
    public class QuartzHostedService : IHostedService
    {
        private readonly ILogger<QuartzHostedService> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        private readonly IJobFactory _jobFactory;
        private IScheduler _scheduler;

        public QuartzHostedService(
            ILogger<QuartzHostedService> logger,
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<JobSchedule> jobSchedules)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("正在启动 Quartz 调度器...");

                _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                _scheduler.JobFactory = _jobFactory;

                var scheduledCount = 0;
                foreach (var jobSchedule in _jobSchedules)
                {
                    try
                    {
                        var job = CreateJob(jobSchedule);
                        var trigger = CreateTrigger(jobSchedule);

                        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
                        scheduledCount++;

                        _logger.LogInformation("✅ 任务已调度: {JobName}, Cron: {CronExpression}",
                            jobSchedule.JobType.Name,
                            jobSchedule.CronExpression);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ 调度任务 '{JobName}' 失败", jobSchedule.JobType.Name);
                        throw;
                    }
                }

                await _scheduler.Start(cancellationToken);

                _logger.LogInformation("🚀 Quartz 调度器已启动，共调度 {Count} 个任务", scheduledCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Quartz 调度器启动失败");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_scheduler != null)
                {
                    _logger.LogInformation("正在停止 Quartz 调度器...");
                    await _scheduler.Shutdown(cancellationToken);
                    _logger.LogInformation("✅ Quartz 调度器已停止");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 停止 Quartz 调度器时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 创建任务详情
        /// </summary>
        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName.ToLower()}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }
}
