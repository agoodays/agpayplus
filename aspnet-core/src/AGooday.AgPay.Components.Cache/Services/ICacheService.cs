namespace AGooday.AgPay.Components.Cache.Services
{
    public interface ICacheService
    {
        #region 基本操作
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存数据</returns>
        Task<T> GetAsync<T>(string key);
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">过期时间</param>
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        /// <summary>
        /// 移除缓存数据
        /// </summary>
        /// <param name="key">缓存键</param>
        Task RemoveAsync(string key);
        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(string key);
        /// <summary>
        /// 更新缓存值，但保持失效时间不变
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">新的缓存值</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateWithExistingExpiryAsync<T>(string key, T value);
        /// <summary>
        /// 根据模式匹配获取缓存键
        /// </summary>
        /// <param name="pattern">匹配模式（如 "user_*"）</param>
        /// <returns>匹配的缓存键列表</returns>
        Task<IEnumerable<string>> GetKeysAsync(string pattern);
        #endregion

        #region 批量操作
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);
        Task SetAllAsync<T>(IDictionary<string, T> keyValuePairs, TimeSpan expiration);
        Task RemoveAllAsync(IEnumerable<string> keys);
        #endregion

        #region 分布式锁
        Task<bool> AcquireLockAsync(string key, TimeSpan expiration);
        Task ReleaseLockAsync(string key);
        Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiration);
        Task ReleaseLockAsync(string key, string value);
        /// <summary>
        /// 异步分布式锁
        /// </summary>
        /// <param name="lockKey">锁名称，不可重复</param>
        /// <param name="lockFunc">任务</param>
        /// <returns>是否成功获取锁并执行任务</returns>
        Task<bool> LockTakeAsync(string lockKey, Func<Task> lockFunc);
        /// <summary>
        /// 同步分布式锁
        /// </summary>
        /// <param name="lockKey">锁名称，不可重复</param>
        /// <param name="action">委托事件</param>
        /// <returns>是否成功获取锁并执行任务</returns>
        bool LockTake(string lockKey, Action action);
        #endregion

        #region 缓存过期策略
        Task SetWithSlidingExpirationAsync<T>(string key, T value, TimeSpan slidingExpiration);
        Task SetWithAbsoluteExpirationAsync<T>(string key, T value, DateTimeOffset absoluteExpiration);
        #endregion
    }
}
