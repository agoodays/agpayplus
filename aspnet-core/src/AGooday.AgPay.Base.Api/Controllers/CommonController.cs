using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Base.Api.Extensions;
using AGooday.AgPay.Base.Api.Extensions.AuthContext;
using AGooday.AgPay.Base.Api.Models;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Base.Api.Controllers
{
    [Authorize]
    public abstract class CommonController : ControllerBase
    {
        protected readonly ILogger<CommonController> _logger;
        protected readonly ICacheService _cacheService;
        protected readonly IAuthService _authService;

        public CommonController(ILogger<CommonController> logger,
            ICacheService cacheService,
            IAuthService authService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _authService = authService;
        }

        protected async Task<CurrentUser> GetCurrentUserAsync()
        {
            if (AuthContextService.CurrentUser.CacheKey == null)
            {
                throw new UnauthorizeException();
                //throw new BizException("登录失效");
            }
            var currentUser = await _cacheService.GetAsync<CurrentUser>(AuthContextService.CurrentUser.CacheKey);
            if (currentUser == null)
            {
                throw new UnauthorizeException();
                //throw new BizException("登录失效");
            }
            return currentUser;
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns></returns>
        protected async Task<long> GetCurrentUserIdAsync() => (await GetCurrentUserAsync()).SysUser.SysUserId;

        /// <summary>
        /// 获取当前商户ID
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetCurrentMchNoAsync() => (await GetCurrentUserAsync()).SysUser.BelongInfoId;

        /// <summary>
        /// 获取当前代理商ID
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetCurrentAgentNoAsync() => (await GetCurrentUserAsync()).SysUser.BelongInfoId;

        /// <summary>
        /// 根据用户ID 删除用户缓存信息
        /// </summary>
        /// <param name="sysUserIdList"></param>
        protected async Task DelAuthenticationAsync(List<long> sysUserIdList)
        {
            if (sysUserIdList == null || sysUserIdList.Count <= 0)
            {
                return;
            }
            foreach (var sysUserId in sysUserIdList)
            {
                var keys = await _cacheService.GetKeysAsync(CS.GetCacheKeyToken(sysUserId, "*"));
                await _cacheService.RemoveAllAsync(keys);
            }
        }

        /// <summary>
        /// 根据用户ID 更新缓存中的权限集合， 使得分配实时生效
        /// </summary>
        /// <param name="sysUserIdList"></param>
        protected async Task RefAuthenticationAsync(List<long> sysUserIdList)
        {
            var sysUserMap = _authService.GetUserByIds(sysUserIdList);
            foreach (var sysUserId in sysUserIdList)
            {
                var keys = await _cacheService.GetKeysAsync(CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in keys)
                {
                    //用户不存在 || 已禁用 需要删除Redis
                    if (!sysUserMap.Any(a => a.SysUserId.Equals(sysUserId))
                    || sysUserMap.Any(a => a.SysUserId.Equals(sysUserId) || a.State.Equals(CS.PUB_DISABLE)))
                    {
                        await _cacheService.RemoveAsync(key);
                        continue;
                    }
                    var currentUser = await _cacheService.GetAsync<CurrentUser>(key);
                    if (currentUser == null)
                    {
                        continue;
                    }
                    var auth = await _authService.GetUserAuthInfoByIdAsync(currentUser.SysUser.SysUserId);
                    var authorities = new List<string>();
                    var ents = new List<SysEntitlementDto>();
                    auth?.GetEnts(_authService, out authorities, out ents);
                    if (auth == null || ents?.Count <= 0)
                    {
                        // 当前用户未分配任何菜单权限，需要删除Redis
                        await _cacheService.RemoveAsync(key);
                        continue;
                    }
                    currentUser.SysUser = auth;
                    currentUser.Authorities = authorities;
                    //保存token  失效时间不变
                    await _cacheService.UpdateWithExistingExpiryAsync(key, currentUser);
                }
            }
        }
    }
}
