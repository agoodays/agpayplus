using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using System.Reflection;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 简单说一下Cron表达式吧，
    ///
    /// 由7段构成：秒 分 时 日 月 星期 年（可选）
    ///
    /// "-" ：表示范围 MON-WED表示星期一到星期三
    /// "," ：表示列举 MON, WEB表示星期一和星期三
    /// "*" ：表是“每”，每月，每天，每周，每年等
    /// "/" :表示增量：0/15（处于分钟段里面） 每15分钟，在0分以后开始，3/20 每20分钟，从3分钟以后开始
    /// "?" :只能出现在日，星期段里面，表示不指定具体的值
    /// "L" :只能出现在日，星期段里面，是Last的缩写，一个月的最后一天，一个星期的最后一天（星期六）
    /// "W" :表示工作日，距离给定值最近的工作日
    /// "#" :表示一个月的第几个星期几，例如："6#3"表示每个月的第三个星期五（1=SUN...6=FRI,7=SAT）
    ///
    /// 如果Minutes的数值是 '0/15' ，表示从0开始每15分钟执行
    ///
    /// 如果Minutes的数值是 '3/20' ，表示从3开始每20分钟执行，也就是‘3/23/43’
    /// https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
    /// </summary>
    public static class QuartzExtensions
    {
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册 Quartz 服务
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // 绑定配置
            var quartzSettings = new QuartzJobSettings();
            configuration.GetSection("Quartz").Bind(quartzSettings);

            // 注册任务
            // 获取当前命名空间
            //string targetNamespace = $"{typeof(QuartzExtensions).Namespace}";
            //foreach (var jobSetting in quartzSettings.Jobs)
            //{
            //    var jobType = Type.GetType($"{targetNamespace}.{jobSetting.JobType}") ?? throw new InvalidOperationException($"Job type '{jobSetting.JobType}' not found.");
            //    services.AddSingleton(jobType);
            //    services.AddSingleton(new JobSchedule(jobType, jobSetting.CronExpression));
            //}

            // 获取当前程序集中所有实现了 IJob 接口的类型
            var jobTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            // 注册任务
            foreach (var jobSetting in quartzSettings.Jobs)
            {
                var jobType = jobTypes.FirstOrDefault(t => t.Name == jobSetting.JobType)
                    ?? throw new InvalidOperationException($"Job type '{jobSetting.JobType}' not found.");

                services.AddSingleton(jobType);
                services.AddSingleton(new JobSchedule(jobType, jobSetting.CronExpression));
            }

            // 注册 HostedService
            services.AddHostedService<QuartzHostedService>();

            return services;
        }
    }
}
