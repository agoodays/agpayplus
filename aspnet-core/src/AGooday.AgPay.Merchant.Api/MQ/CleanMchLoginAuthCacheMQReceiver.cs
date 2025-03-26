using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.MQ.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Merchant.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：清除商户登录信息
    /// </summary>
    public class CleanMchLoginAuthCacheMQReceiver : CleanMchLoginAuthCacheMQ.IMQReceiver
    {
        private readonly ILogger<CleanMchLoginAuthCacheMQReceiver> _logger;
        private readonly ICacheService _cacheService;

        public CleanMchLoginAuthCacheMQReceiver(ILogger<CleanMchLoginAuthCacheMQReceiver> logger,
            ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task ReceiveAsync(CleanMchLoginAuthCacheMQ.MsgPayload payload)
        {
            _logger.LogInformation("成功接收删除商户用户登录的订阅通知, msg={payload}", JsonConvert.SerializeObject(payload));
            //_logger.LogInformation($"成功接收删除商户用户登录的订阅通知, msg={JsonConvert.SerializeObject(payload)}");
            // 字符串转List<Long>
            List<long> userIdList = payload.UserIdList;
            // 删除redis用户缓存
            if (userIdList == null || userIdList.Count == 0)
            {
                _logger.LogInformation("用户ID为空");
                return;
            }
            foreach (long sysUserId in userIdList)
            {
                var keys = await _cacheService.GetKeysAsync(CS.GetCacheKeyToken(sysUserId, "*"));
                if (keys == null || !keys.Any())
                {
                    continue;
                }
                // 删除用户Redis信息
                await _cacheService.RemoveAllAsync(keys);
            }
            _logger.LogInformation("无权限登录用户信息已清除");
        }
    }
}
