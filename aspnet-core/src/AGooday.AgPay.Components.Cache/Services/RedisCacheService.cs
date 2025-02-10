using AGooday.AgPay.Components.Cache.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Components.Cache.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly int _defaultDatabase;

        public RedisCacheService(
            IDistributedCache distributedCache,
            IConnectionMultiplexer redisConnection,
            IOptions<RedisOptions> redisOptions)
        {
            _distributedCache = distributedCache;
            _redisConnection = redisConnection;
            _defaultDatabase = redisOptions.Value.DefaultDB;
        }

        #region 基本操作
        public async Task<T> GetAsync<T>(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            // 如果 T 是 string 类型，直接返回 cachedData
            if (typeof(T) == typeof(string))
            {
                return (T)(object)cachedData;
            }
            // 否则反序列化为 T 类型
            //return cachedData == null ? default : JsonSerializer.Deserialize<T>(cachedData);
            return cachedData == null ? default : JsonConvert.DeserializeObject<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            string valueToCache;

            // 如果 T 是 string 类型，直接存储 value
            if (typeof(T) == typeof(string))
            {
                valueToCache = value as string;
            }
            else
            {
                // 否则序列化为 JSON 字符串
                //valueToCache = JsonSerializer.Serialize(value);
                valueToCache = JsonConvert.SerializeObject(value);
            }

            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
            await _distributedCache.SetStringAsync(key, valueToCache, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            return cachedData != null;
        }

        /// <summary>
        /// 更新缓存值，但保持失效时间不变
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">新的缓存值</param>
        /// <returns>是否更新成功</returns>
        public async Task<bool> UpdateWithExistingExpiryAsync<T>(string key, T value)
        {
            var database = _redisConnection.GetDatabase(_defaultDatabase);

            // 获取当前缓存的剩余过期时间
            var cacheExpiry = await database.KeyTimeToLiveAsync(key);
            if (cacheExpiry == null)
            {
                // 如果缓存不存在或没有设置过期时间，直接返回 false
                return false;
            }

            // 更新缓存值，保持原有的过期时间
            //var valueJson = JsonSerializer.Serialize(value);
            var valueJson = JsonConvert.SerializeObject(value);
            return await database.StringSetAsync(key, valueJson, cacheExpiry, When.Exists);
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
        {
            var database = _redisConnection.GetDatabase(_defaultDatabase);
            var endpoints = _redisConnection.GetEndPoints();
            var keys = new List<string>();

            foreach (var endpoint in endpoints)
            {
                var server = _redisConnection.GetServer(endpoint);
                var serverKeys = server.Keys(_defaultDatabase, pattern);
                keys.AddRange(serverKeys.Select(key => key.ToString()));
            }

            return keys;
        }
        #endregion

        #region 批量操作
        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var value = await GetAsync<T>(key);
                if (value != null)
                {
                    result[key] = value;
                }
            }
            return result;
        }

        public async Task SetAllAsync<T>(IDictionary<string, T> keyValuePairs, TimeSpan expiration)
        {
            foreach (var kvp in keyValuePairs)
            {
                await SetAsync(kvp.Key, kvp.Value, expiration);
            }
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }
        }
        #endregion

        #region 分布式锁
        public async Task<bool> AcquireLockAsync(string key, TimeSpan expiration)
        {
            var database = GetDatabase();
            return await database.StringSetAsync(key, "locked", expiration, When.NotExists);
        }

        public async Task ReleaseLockAsync(string key)
        {
            var database = GetDatabase();
            await database.KeyDeleteAsync(key);
        }

        public async Task<bool> ExecLockTakeAsync(string lockKey, Func<Task> lockFunc)
        {
            var result = false;
            var lockAcquired = await this.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(10));

            if (lockAcquired)
            {
                try
                {
                    // 执行需要加锁的操作
                    await lockFunc();
                    result = true;
                }
                finally
                {
                    // 释放锁
                    await this.ReleaseLockAsync(lockKey);
                }
            }

            return result;
        }

        /// <summary>
        /// 异步分布式锁
        /// </summary>
        /// <param name="lockKey">锁名称，不可重复</param>
        /// <param name="lockFunc">任务</param>
        /// <returns>是否成功获取锁并执行任务</returns>
        public async Task<bool> LockTakeAsync(string lockKey, Func<Task> lockFunc)
        {
            var result = false;
            var database = GetDatabase();

            // token 用来标识谁拥有该锁并用来释放锁
            RedisValue token = Environment.MachineName;

            // TimeSpan 表示该锁的有效时间。10秒后自动释放，避免死锁
            if (await database.LockTakeAsync(lockKey, token, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    await lockFunc();
                    result = true;
                }
                finally
                {
                    await database.LockReleaseAsync(lockKey, token); // 释放锁
                }
            }

            return result;
        }

        /// <summary>
        /// 同步分布式锁
        /// </summary>
        /// <param name="lockKey">锁名称，不可重复</param>
        /// <param name="action">委托事件</param>
        /// <returns>是否成功获取锁并执行任务</returns>
        public bool LockTake(string lockKey, Action action)
        {
            var result = false;
            var database = GetDatabase();

            // token 用来标识谁拥有该锁并用来释放锁
            RedisValue token = Environment.MachineName;

            // TimeSpan 表示该锁的有效时间。10秒后自动释放，避免死锁
            if (database.LockTake(lockKey, token, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    action();
                    result = true;
                }
                finally
                {
                    database.LockRelease(lockKey, token); // 释放锁
                }
            }

            return result;
        }
        #endregion

        #region 缓存过期策略
        public async Task SetWithSlidingExpirationAsync<T>(string key, T value, TimeSpan slidingExpiration)
        {
            var options = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration };
            //await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(value), options);
        }

        public async Task SetWithAbsoluteExpirationAsync<T>(string key, T value, DateTimeOffset absoluteExpiration)
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration };
            //await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(value), options);
        }
        #endregion

        /// <summary>
        /// 获取 Redis 数据库实例
        /// </summary>
        /// <returns></returns>
        private IDatabase GetDatabase()
        {
            return _redisConnection.GetDatabase(_defaultDatabase);
        }
    }
}
