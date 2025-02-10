using AGooday.AgPay.Agent.Api.Extensions;
using AGooday.AgPay.Agent.Api.Extensions.AuthContext;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers
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
                //throw new BizException("��¼ʧЧ");
            }
            var currentUser = await _cacheService.GetAsync<CurrentUser>(AuthContextService.CurrentUser.CacheKey);
            if (currentUser == null)
            {
                throw new UnauthorizeException();
                //throw new BizException("��¼ʧЧ");
            }
            return currentUser;
        }

        /// <summary>
        /// ��ȡ��ǰ�û�ID
        /// </summary>
        /// <returns></returns>
        protected async Task<long> GetCurrentUserIdAsync() => (await GetCurrentUserAsync()).SysUser.SysUserId;

        /// <summary>
        /// ��ȡ��ǰ������ID
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetCurrentAgentNoAsync() => (await GetCurrentUserAsync()).SysUser.BelongInfoId;

        /// <summary>
        /// �����û�ID ɾ���û�������Ϣ
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
        /// �����û�ID ���»����е�Ȩ�޼��ϣ� ʹ�÷���ʵʱ��Ч
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
                    //�û������� || �ѽ��� ��Ҫɾ��Redis
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
                        // ��ǰ�û�δ�����κβ˵�Ȩ�ޣ���Ҫɾ��Redis
                        await _cacheService.RemoveAsync(key);
                        continue;
                    }
                    currentUser.SysUser = auth;
                    currentUser.Authorities = authorities;
                    //����token  ʧЧʱ�䲻��
                    await _cacheService.UpdateWithExistingExpiryAsync(key, currentUser);
                }
            }
        }
    }
}