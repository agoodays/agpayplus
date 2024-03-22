using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Merchant.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：清除代理商登录信息
    /// </summary>
    public class CleanAgentLoginAuthCacheMQReceiver : CleanAgentLoginAuthCacheMQ.IMQReceiver
    {
        private readonly ILogger<CleanAgentLoginAuthCacheMQReceiver> log;
        private readonly int defaultDB;
        private readonly IDatabase redis;
        private readonly IServer redisServer;

        public CleanAgentLoginAuthCacheMQReceiver(ILogger<CleanAgentLoginAuthCacheMQReceiver> log, RedisUtil client)
        {
            this.log = log;
            defaultDB = client.GetDefaultDB();
            redis = client.GetDatabase();
            redisServer = client.GetServer();
        }

        public void Receive(CleanAgentLoginAuthCacheMQ.MsgPayload payload)
        {
            log.LogInformation($"成功接收删除代理商用户登录的订阅通知, msg={JsonConvert.SerializeObject(payload)}");
            // 字符串转List<Long>
            List<long> userIdList = payload.UserIdList;
            // 删除redis用户缓存
            if (userIdList == null || userIdList.Count == 0)
            {
                log.LogInformation("用户ID为空");
                return;
            }
            foreach (long sysUserId in userIdList)
            {
                var cacheKeyList = redisServer.Keys(defaultDB, CS.GetCacheKeyToken(sysUserId, "*"));
                if (cacheKeyList == null || !cacheKeyList.Any())
                {
                    continue;
                }
                foreach (string cacheKey in cacheKeyList)
                {
                    // 删除用户Redis信息
                    redis.KeyDelete(cacheKey);
                    continue;
                }
            }
            log.LogInformation("无权限登录用户信息已清除");
        }
    }
}
