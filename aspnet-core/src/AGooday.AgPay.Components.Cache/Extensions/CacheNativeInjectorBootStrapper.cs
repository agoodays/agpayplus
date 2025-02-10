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
            // 注册 Redis 分布式缓存
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisOptions.Connection; // Redis 服务器地址
                options.InstanceName = redisOptions.InstanceName; // 缓存键前缀
            });

            // 注册 Redis 连接
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions.Connection));

            // 注册缓存服务
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
    }
}
