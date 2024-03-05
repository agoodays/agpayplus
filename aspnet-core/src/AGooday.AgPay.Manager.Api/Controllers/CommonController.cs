using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
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
        private readonly ILogger<CommonController> _logger;
        private readonly int defaultDB;
        private readonly IDatabase redis;
        private readonly IServer redisServer;
        private readonly ISysUserService _sysUserService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public CommonController(ILogger<CommonController> logger, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
        {
            _logger = logger;
            defaultDB = client.GetDefaultDB();
            redis = client.GetDatabase();
            redisServer = client.GetServer();
            _sysUserService = sysUserService;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
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
            var sysUserMap = _sysUserService.GetByIds(sysUserIdList);
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
                    string currentUserJson = redis.StringGet(AuthContextService.CurrentUser.CacheKey);
                    var currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserJson);
                    if (currentUser == null)
                    {
                        continue;
                    }
                    var auth = sysUserMap.Where(w => w.SysUserId.Equals(sysUserId)).First();
                    var authorities = _sysUserRoleRelaService.SelectRoleIdsByUserId(auth.SysUserId.Value).ToList();
                    authorities.AddRange(_sysRoleEntRelaService.SelectEntIdsByUserId(auth.SysUserId.Value, auth.UserType, auth.SysType));
                    currentUser.Authorities = authorities;
                    currentUserJson = JsonConvert.SerializeObject(currentUser);
                    //保存token  失效时间不变
                    redis.StringSet(key, currentUserJson);
                }
            });
        }
    }
}
