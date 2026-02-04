using System.Collections.Specialized;
using System.Reflection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// Quartz 扩展方法
    /// 
    /// Cron 表达式说明：
    /// 由 7 段构成：秒 分 时 日 月 星期 年（可选）
    ///
    /// "-" ：表示范围 MON-WED表示星期一到星期三
    /// "," ：表示列举 MON, WEB表示星期一和星期三
    /// "*" ：表示"每"，每月，每天，每周，每年等
    /// "/" ：表示增量：0/15（处于分钟段里面）每15分钟，在0分以后开始，3/20 每20分钟，从3分钟以后开始
    /// "?" ：只能出现在日，星期段里面，表示不指定具体的值
    /// "L" ：只能出现在日，星期段里面，是Last的缩写，一个月的最后一天，一个星期的最后一天（星期六）
    /// "W" ：表示工作日，距离给定值最近的工作日
    /// "#" ：表示一个月的第几个星期几，例如："6#3"表示每个月的第三个星期五（1=SUN...6=FRI,7=SAT）
    ///
    /// 示例：
    /// - "0 0/15 * * * ?" 表示从0分开始每15分钟执行
    /// - "0 3/20 * * * ?" 表示从3分开始每20分钟执行（3,23,43分）
    /// 
    /// 参考：https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
    /// </summary>
    public static class QuartzExtensions
    {
        /// <summary>
        /// 添加 Quartz 任务调度服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置对象</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. 读取 Quartz 配置
            var quartzConfig = BuildQuartzConfiguration(configuration);

            // 2. 注册 Quartz 核心服务
            services.AddSingleton<IJobFactory, DependencyInjectionJobFactory>();
            services.AddSingleton<ISchedulerFactory>(provider =>
            {
                var logger = provider.GetService<ILogger<StdSchedulerFactory>>();
                var schedulerFactory = new StdSchedulerFactory(quartzConfig);

                // 记录配置信息
                logger?.LogInformation("Quartz 调度器工厂已创建，配置项数量: {Count}", quartzConfig.Count);

                // 检查是否启用集群模式
                var isClusterMode = string.Equals(quartzConfig["quartz.jobStore.clustered"], "true", StringComparison.OrdinalIgnoreCase);
                if (isClusterMode)
                {
                    logger?.LogInformation("? Quartz 集群模式已启用");
                }

                return schedulerFactory;
            });

            // 3. 绑定任务配置
            var quartzSettings = new QuartzJobSettings();
            configuration.GetSection("Quartz").Bind(quartzSettings);

            // 4. 注册任务
            RegisterJobs(services, quartzSettings);

            // 5. 注册 HostedService
            services.AddHostedService<QuartzHostedService>();

            return services;
        }

        /// <summary>
        /// 构建 Quartz 配置
        /// </summary>
        private static NameValueCollection BuildQuartzConfiguration(IConfiguration configuration)
        {
            var config = new NameValueCollection();
            var quartzSection = configuration.GetSection("Quartz");

            // 遍历配置节，读取所有 Quartz 配置
            foreach (var child in quartzSection.GetChildren())
            {
                // 跳过 Jobs 配置（这个是自定义配置，不是 Quartz 原生配置）
                if (child.Key == "Jobs")
                    continue;

                // 如果是简单值，直接添加
                if (child.Value != null)
                {
                    config[child.Key] = child.Value;
                }
                // 如果是嵌套对象，递归处理
                else
                {
                    AddNestedConfiguration(config, child.Key, child);
                }
            }

            // 如果没有配置，使用默认配置
            if (config.Count == 0)
            {
                config["quartz.scheduler.instanceName"] = "DefaultScheduler";
                config["quartz.scheduler.instanceId"] = "AUTO";
                config["quartz.threadPool.type"] = "Quartz.Simpl.DefaultThreadPool, Quartz";
                config["quartz.threadPool.threadCount"] = "10";
                config["quartz.threadPool.threadPriority"] = "Normal";
            }

            return config;
        }

        /// <summary>
        /// 递归添加嵌套配置
        /// </summary>
        private static void AddNestedConfiguration(NameValueCollection config, string prefix, IConfigurationSection section)
        {
            foreach (var child in section.GetChildren())
            {
                var key = $"{prefix}.{child.Key}";
                if (child.Value != null)
                {
                    config[key] = child.Value;
                }
                else
                {
                    AddNestedConfiguration(config, key, child);
                }
            }
        }

        /// <summary>
        /// 注册任务
        /// </summary>
        private static void RegisterJobs(IServiceCollection services, QuartzJobSettings quartzSettings)
        {
            if (quartzSettings.Jobs == null || quartzSettings.Jobs.Count == 0)
            {
                throw new InvalidOperationException("Quartz 配置中未找到任何任务定义（Quartz:Jobs）。");
            }

            // 获取当前程序集中所有实现了 IJob 接口的类型
            var jobTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            var logger = services.BuildServiceProvider().GetService<ILogger<QuartzHostedService>>();

            logger?.LogInformation("开始注册 Quartz 任务，配置任务数: {Count}", quartzSettings.Jobs.Count);

            // 验证是否有重复的任务
            var duplicates = quartzSettings.Jobs
                .GroupBy(j => j.JobType)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                throw new InvalidOperationException(
                    $"? 配置中存在重复的任务: {string.Join(", ", duplicates)}");
            }

            // 注册任务
            foreach (var jobSetting in quartzSettings.Jobs)
            {
                try
                {
                    // 1. 查找任务类型
                    var jobType = jobTypes.FirstOrDefault(t => t.Name == jobSetting.JobType);
                    if (jobType == null)
                    {
                        var availableJobs = string.Join(", ", jobTypes.Select(t => t.Name));
                        throw new InvalidOperationException(
                            $"? 任务类型 '{jobSetting.JobType}' 未找到。可用的任务类型: {availableJobs}");
                    }

                    // 2. 验证 Cron 表达式
                    if (!CronExpression.IsValidExpression(jobSetting.CronExpression))
                    {
                        throw new InvalidOperationException(
                            $"? 任务 '{jobSetting.JobType}' 的 Cron 表达式 '{jobSetting.CronExpression}' 无效。");
                    }


                    // 3. 注册任务实例（使用 Transient 生命周期，每次执行创建新实例）
                    services.AddTransient(jobType);

                    // 4. 注册任务调度计划
                    services.AddSingleton(new JobSchedule(jobType, jobSetting.CronExpression));

                    // 5. 记录日志
                    logger?.LogInformation("? 任务已注册 (Transient): {JobType}, Cron: {CronExpression}, 描述: {Description}",
                        jobSetting.JobType,
                        jobSetting.CronExpression,
                        jobSetting.Description ?? "无");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "? 注册任务 '{JobType}' 失败", jobSetting.JobType);
                    throw;
                }
            }

            logger?.LogInformation("Quartz 任务注册完成，共注册 {Count} 个任务", quartzSettings.Jobs.Count);
        }
    }
}
