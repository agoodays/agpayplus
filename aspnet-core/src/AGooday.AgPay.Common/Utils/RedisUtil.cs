using System.Collections.Concurrent;
using StackExchange.Redis;

namespace AGooday.AgPay.Common.Utils
{
    public class RedisUtil
    {
        //连接字符串
        private readonly string _connectionString;
        //实例名称
        private readonly string _instanceName;
        //默认数据库
        private readonly int _defaultDB;
        private readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections;
        public RedisUtil(string connectionString, string instanceName, int defaultDB = 0)
        {
            _connectionString = connectionString;
            _instanceName = instanceName;
            _defaultDB = defaultDB;
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnect()
        {
            return _connections.GetOrAdd(_instanceName, p => ConnectionMultiplexer.Connect(_connectionString));
        }

        public int GetDefaultDB()
        {
            return _defaultDB;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="db">默认为0：优先代码的db配置，其次config中的配置</param>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return GetConnect().GetDatabase(_defaultDB);
        }

        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            var confOption = ConfigurationOptions.Parse(_connectionString);
            return GetConnect().GetServer(confOption.EndPoints[endPointsIndex]);
        }

        public ISubscriber GetSubscriber(string configName = null)
        {
            return GetConnect().GetSubscriber();
        }

        /// <summary>
        /// 根据匹配表达式获取RedisKey
        /// </summary>
        /// <param name="pattern">匹配表达式</param>
        /// <returns></returns>
        public IEnumerable<RedisKey> GetRedisKeysByPattern(string pattern)
        {
            var multiplexer = GetConnect();
            var database = multiplexer.GetDatabase();
            foreach (var endPoint in multiplexer.GetEndPoints())
            {
                var _server = multiplexer.GetServer(endPoint);
                IEnumerable<RedisKey> keys = _server.Keys(database.Database, pattern);
                return keys;
            }
            multiplexer.Close();
            return null;
        }

        /// <summary>
        /// 异步分布式锁
        /// </summary>
        /// <param name="lockKey">锁名称，不可重复</param>
        /// <param name="lockFunc">任务</param>
        /// <returns>bool</returns>
        public async Task<bool> LockTakeAsync(string lockKey, Func<Task> lockFunc)
        {
            var result = false;

            var database = GetDatabase();
            //token用来标识谁拥有该锁并用来释放锁。
            RedisValue token = Environment.MachineName;
            //TimeSpan表示该锁的有效时间。10秒后自动释放，避免死锁。
            if (database.LockTake(lockKey, token, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    await lockFunc();
                    result = true;
                }
                finally
                {
                    database.LockRelease(lockKey, token);//释放锁
                    Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="lockKey">锁名称，不可重复</param>
        /// <param name="action">委托事件</param>
        /// <returns>bool</returns>
        public bool LockTake(string lockKey, Action action)
        {
            var result = false;

            var database = GetDatabase();
            //token用来标识谁拥有该锁并用来释放锁。
            RedisValue token = Environment.MachineName;
            //TimeSpan表示该锁的有效时间。10秒后自动释放，避免死锁。
            if (database.LockTake(lockKey, token, TimeSpan.FromSeconds(10)))
            {
                try
                {
                    action();
                    result = true;
                }
                finally
                {
                    database.LockRelease(lockKey, token);//释放锁
                    Dispose();
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (_connections != null && !_connections.IsEmpty)
            {
                foreach (var item in _connections.Values)
                {
                    item.Close();
                }
            }
        }
    }
}
