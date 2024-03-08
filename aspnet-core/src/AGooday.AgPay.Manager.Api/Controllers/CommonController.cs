using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.Extensions.AuthContext;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Authorize]
    public abstract class CommonController : ControllerBase
    {
        protected readonly ILogger<CommonController> _logger;
        private readonly int defaultDB;
        private readonly IDatabase redis;
        private readonly IServer redisServer;
        private readonly IAuthService _authService;

        public CommonController(ILogger<CommonController> logger, 
            RedisUtil client,
            IAuthService authService)
        {
            _logger = logger;
            defaultDB = client.GetDefaultDB();
            redis = client.GetDatabase();
            redisServer = client.GetServer();
            _authService = authService;
        }

        protected CurrentUser GetCurrentUser()
        {
            if (AuthContextService.CurrentUser.CacheKey == null)
            {
                throw new UnauthorizeException();
                //throw new BizException("登录失效");
            }
            string currentUser = redis.StringGet(AuthContextService.CurrentUser.CacheKey);
            if (currentUser == null)
            {
                throw new UnauthorizeException();
                //throw new BizException("登录失效");
            }
            return JsonConvert.DeserializeObject<CurrentUser>(currentUser);
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns></returns>
        protected long GetCurrentUserId()
        {
            return GetCurrentUser().SysUser.SysUserId;
        }

        /// <summary>
        /// 根据用户ID 删除用户缓存信息
        /// </summary>
        /// <param name="sysUserIdList"></param>
        protected void DelAuthentication(List<long> sysUserIdList)
        {
            if (sysUserIdList == null || sysUserIdList.Count<=0)
            {
                return;
            }
            foreach (var sysUserId in sysUserIdList)
            {
                var redisKeys = redisServer.Keys(defaultDB, CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in redisKeys)
                {
                    redis.KeyDelete(key);
                }
            }
        }

        /// <summary>
        /// 根据用户ID 更新缓存中的权限集合， 使得分配实时生效
        /// </summary>
        /// <param name="sysUserIdList"></param>
        protected void RefAuthentication(List<long> sysUserIdList)
        {
            var sysUserMap = _authService.GetUserByIds(sysUserIdList);
            sysUserIdList.ForEach(sysUserId =>
            {
                var redisKeys = redisServer.Keys(defaultDB, CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in redisKeys)
                {
                    //用户不存在 || 已禁用 需要删除Redis
                    if (!sysUserMap.Any(a => a.SysUserId.Equals(sysUserId))
                    || sysUserMap.Any(a => a.SysUserId.Equals(sysUserId) || a.State.Equals(CS.PUB_DISABLE)))
                    {
                        redis.KeyDelete(key);
                        continue;
                    }
                    string currentUserJson = redis.StringGet(key);
                    var currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserJson);
                    if (currentUser == null)
                    {
                        continue;
                    }
                    var auth = _authService.GetUserAuthInfoById(currentUser.SysUser.SysUserId);
                    var authorities = new List<string>();
                    var ents = new List<SysEntitlementDto>();
                    auth?.GetEnts(_authService, out authorities, out ents);
                    if (auth == null || ents?.Count <= 0)
                    {
                        // 当前用户未分配任何菜单权限，需要删除Redis
                        redis.KeyDelete(key);
                        continue;
                    }
                    currentUser.SysUser = auth;
                    currentUser.Authorities = authorities;
                    currentUserJson = JsonConvert.SerializeObject(currentUser);
                    //保存token  失效时间不变
                    var cacheExpiry = redis.KeyTimeToLive(key);
                    redis.StringSet(key, currentUserJson, cacheExpiry, When.Exists);
                }
            });
        }
    }
}
