using AGooday.AgPay.Components.Cache.Options;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AGooday.AgPay.Components.Cache.Extensions
{
    public class CacheNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services, RedisOptions redisOptions)
        {
            // 创建统一连接实例（支持重试和超时）
            var config = ConfigurationOptions.Parse(redisOptions.Connection);
            config.AbortOnConnectFail = false;
            config.ConnectTimeout = 3000;            // 连接超时（毫秒）
            config.SyncTimeout = 3000;               // 同步操作超时
            //config.AsyncTimeout = 5000;            // 异步操作超时
            //config.KeepAlive = 60;                 // 心跳保活（秒）
            //config.ReconnectRetryPolicy = new ExponentialRetry(1000); // 重试策略
            //config.AbortOnConnectFail = false;     // 禁用快速失败
            //config.AllowAdmin = true;  // 允许查看服务器状态
            var redisConnection = ConnectionMultiplexer.Connect(config);

            // 注册单例 ConnectionMultiplexer
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);

            // 替换 AddStackExchangeRedisCache 的默认工厂
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = string.Empty;
                options.InstanceName = redisOptions.InstanceName;
                options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(redisConnection);
            });

            // 注册缓存服务
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
    }
}
