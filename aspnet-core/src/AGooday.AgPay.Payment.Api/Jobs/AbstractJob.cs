using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Cache.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    public abstract class AbstractJob : IJob
    {
        protected readonly ILogger<AbstractJob> _logger;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ICacheService _cacheService;

        protected AbstractJob(ILogger<AbstractJob> logger,
            IServiceScopeFactory serviceScopeFactory,
            ICacheService cacheService)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _cacheService = cacheService;
        }

        public abstract Task Execute(IJobExecutionContext context);

        protected string GetJobName() => GetType().Name;

        protected async Task<bool> ExecuteLockTakeAsync(string jobKey, Func<Task> lockFunc)
        {
            var lockExpiry = TimeSpan.FromMinutes(5);
            return await ExecuteLockTakeAsync(lockExpiry, jobKey, lockFunc);
        }

        protected async Task<bool> ExecuteLockTakeAsync(TimeSpan lockExpiry, string jobKey, Func<Task> lockFunc)
        {
            var taskName = GetJobName();
            var lockKey = CS.GetCacheKeyTaskLock(taskName);
            var lockValue = Guid.NewGuid().ToString();
            return await ExecuteLockTakeAsync(lockKey, lockValue, lockExpiry, jobKey, lockFunc);
        }

        protected async Task<bool> ExecuteLockTakeAsync(string lockKey, string lockValue, TimeSpan lockExpiry, string jobKey, Func<Task> lockFunc)
        {
            var result = false;

            try
            {
                // 尝试获取分布式锁
                if (!await _cacheService.AcquireLockAsync(lockKey, lockValue, lockExpiry))
                {
                    _logger.LogWarning("任务 [{JobKey}] 已在其他实例上执行，跳过本次执行。", jobKey);
                    return result;
                }

                // 执行需要加锁的操作
                await lockFunc();
                result = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "任务 [{JobKey}] 执行时发生异常。", jobKey);
            }
            finally
            {
                // 释放分布式锁
                await _cacheService.ReleaseLockAsync(lockKey, lockValue);
            }

            return result;
        }
    }
}
