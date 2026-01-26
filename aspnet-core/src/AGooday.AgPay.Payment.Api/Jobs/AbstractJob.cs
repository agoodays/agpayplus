using System.Diagnostics;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Cache.Services;
using Quartz;
using StackExchange.Redis;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 定时任务抽象基类（支持分布式部署）
    /// </summary>
    public abstract class AbstractJob : IJob
    {
        protected readonly ILogger<AbstractJob> _logger;
        protected readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数（推荐：直接注入 Scoped 服务）
        /// </summary>
        protected AbstractJob(ILogger<AbstractJob> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public abstract Task Execute(IJobExecutionContext context);

        /// <summary>
        /// 获取任务名称
        /// </summary>
        protected string GetJobName() => GetType().Name;

        /// <summary>
        /// 获取分布式锁的超时时间（子类可重写）
        /// </summary>
        protected virtual TimeSpan GetLockExpiry() => TimeSpan.FromMinutes(5);

        /// <summary>
        /// 获取任务的最大执行时间（子类可重写）
        /// </summary>
        protected virtual TimeSpan GetMaxExecutionTime() => TimeSpan.FromMinutes(10);

        /// <summary>
        /// 是否在 Redis 不可用时降级执行（子类可重写）
        /// 默认 false：保守策略，Redis 故障时不执行
        /// </summary>
        protected virtual bool AllowFallbackExecution() => false;

        /// <summary>
        /// 执行带分布式锁保护的任务（简化版）
        /// </summary>
        protected async Task<bool> ExecuteLockTakeAsync(string jobKey, Func<Task> lockFunc)
        {
            var lockExpiry = GetLockExpiry();
            var taskName = GetJobName();
            var lockKey = CS.GetCacheKeyTaskLock(taskName);
            var lockValue = Guid.NewGuid().ToString();

            return await ExecuteLockTakeAsync(lockKey, lockValue, lockExpiry, jobKey, lockFunc);
        }

        /// <summary>
        /// 执行带分布式锁保护的任务（自定义超时时间）
        /// </summary>
        protected async Task<bool> ExecuteLockTakeAsync(TimeSpan lockExpiry, string jobKey, Func<Task> lockFunc)
        {
            var taskName = GetJobName();
            var lockKey = CS.GetCacheKeyTaskLock(taskName);
            var lockValue = Guid.NewGuid().ToString();

            return await ExecuteLockTakeAsync(lockKey, lockValue, lockExpiry, jobKey, lockFunc);
        }

        /// <summary>
        /// 执行带分布式锁保护的任务（完整版，支持故障降级和超时控制）
        /// </summary>
        protected async Task<bool> ExecuteLockTakeAsync(
            string lockKey,
            string lockValue,
            TimeSpan lockExpiry,
            string jobKey,
            Func<Task> lockFunc)
        {
            var result = false;
            var sw = Stopwatch.StartNew();
            using var cts = new CancellationTokenSource(GetMaxExecutionTime());

            try
            {
                // ========== 1. 尝试获取分布式锁 ==========
                bool lockAcquired = false;
                try
                {
                    lockAcquired = await _cacheService.AcquireLockAsync(lockKey, lockValue, lockExpiry);
                }
                catch (RedisConnectionException ex)
                {
                    _logger.LogError(ex, "任务 [{JobKey}] 无法连接 Redis，锁获取失败。", jobKey);
                    return await HandleRedisFailure(jobKey, lockFunc);
                }
                catch (TimeoutException ex)
                {
                    _logger.LogError(ex, "任务 [{JobKey}] Redis 操作超时，锁获取失败。", jobKey);
                    return await HandleRedisFailure(jobKey, lockFunc);
                }

                if (!lockAcquired)
                {
                    _logger.LogInformation("任务 [{JobKey}] 已在其他实例上执行，跳过本次执行。", jobKey);
                    return false;
                }

                // ========== 2. 执行业务逻辑（带超时保护）==========
                _logger.LogInformation("任务 [{JobKey}] 开始执行，锁超时: {LockExpiry}，最大执行时间: {MaxExecutionTime}",
                    jobKey, lockExpiry, GetMaxExecutionTime());

                await ExecuteWithTimeout(lockFunc, cts.Token);

                result = true;
                _logger.LogInformation("任务 [{JobKey}] 执行成功，耗时: {ElapsedMs}ms", jobKey, sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("任务 [{JobKey}] 执行超时（>{MaxExecutionTime}），已强制取消。",
                    jobKey, GetMaxExecutionTime());

                // 执行超时时的清理工作（子类可重写）
                await OnExecutionTimeout(jobKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "任务 [{JobKey}] 执行时发生异常，耗时: {ElapsedMs}ms",
                    jobKey, sw.ElapsedMilliseconds);

                // 执行失败时的处理（子类可重写）
                await OnExecutionFailed(jobKey, ex);
            }
            finally
            {
                // ========== 3. 释放分布式锁 ==========
                await ReleaseLockSafely(lockKey, lockValue, jobKey);

                sw.Stop();
            }

            return result;
        }

        /// <summary>
        /// 处理 Redis 故障的降级策略
        /// </summary>
        private async Task<bool> HandleRedisFailure(string jobKey, Func<Task> lockFunc)
        {
            if (AllowFallbackExecution())
            {
                _logger.LogWarning("任务 [{JobKey}] Redis 不可用，但启用了降级执行策略，将直接执行（⚠️ 可能导致多实例重复执行）。", jobKey);

                try
                {
                    await lockFunc();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "任务 [{JobKey}] 降级执行失败。", jobKey);
                    return false;
                }
            }
            else
            {
                _logger.LogWarning("任务 [{JobKey}] Redis 不可用且未启用降级执行，本次跳过执行（保守策略）。", jobKey);
                return false;
            }
        }

        /// <summary>
        /// 执行带超时控制的任务
        /// </summary>
        private async Task ExecuteWithTimeout(Func<Task> lockFunc, CancellationToken cancellationToken)
        {
            // .NET 6+ 支持 WaitAsync
            await lockFunc().WaitAsync(cancellationToken);
        }

        /// <summary>
        /// 安全释放分布式锁
        /// </summary>
        private async Task ReleaseLockSafely(string lockKey, string lockValue, string jobKey)
        {
            try
            {
                await _cacheService.ReleaseLockAsync(lockKey, lockValue);
                _logger.LogDebug("任务 [{JobKey}] 锁已释放: {LockKey}", jobKey, lockKey);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "任务 [{JobKey}] 释放锁时 Redis 连接失败（锁可能已超时自动释放）。", jobKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "任务 [{JobKey}] 释放锁时发生异常（锁可能已超时自动释放）。", jobKey);
            }
        }

        /// <summary>
        /// 任务执行超时时的回调（子类可重写）
        /// </summary>
        protected virtual Task OnExecutionTimeout(string jobKey)
        {
            // 默认不做处理，子类可重写来清理资源或记录指标
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务执行失败时的回调（子类可重写）
        /// </summary>
        protected virtual Task OnExecutionFailed(string jobKey, Exception exception)
        {
            // 默认不做处理，子类可重写来记录指标或发送告警
            return Task.CompletedTask;
        }
    }
}
