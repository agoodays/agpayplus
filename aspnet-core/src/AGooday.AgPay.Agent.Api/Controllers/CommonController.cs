using AGooday.AgPay.Agent.Api.Extensions;
using AGooday.AgPay.Agent.Api.Extensions.AuthContext;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Agent.Api.Controllers
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
                //throw new BizException("��¼ʧЧ");
            }
            string currentUser = redis.StringGet(AuthContextService.CurrentUser.CacheKey);
            if (currentUser == null)
            {
                throw new UnauthorizeException();
                //throw new BizException("��¼ʧЧ");
            }
            return JsonConvert.DeserializeObject<CurrentUser>(currentUser);
        }

        /// <summary>
        /// ��ȡ��ǰ�û�ID
        /// </summary>
        /// <returns></returns>
        protected long GetCurrentUserId()
        {
            return GetCurrentUser().SysUser.SysUserId;
        }

        /// <summary>
        /// ��ȡ��ǰ������ID
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentAgentNo()
        {
            return GetCurrentUser().SysUser.BelongInfoId;
        }

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
                var redisKeys = redisServer.Keys(defaultDB, CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in redisKeys)
                {
                    await redis.KeyDeleteAsync(key);
                }
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
                var redisKeys = redisServer.Keys(defaultDB, CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in redisKeys)
                {
                    //�û������� || �ѽ��� ��Ҫɾ��Redis
                    if (!sysUserMap.Any(a => a.SysUserId.Equals(sysUserId))
                    || sysUserMap.Any(a => a.SysUserId.Equals(sysUserId) || a.State.Equals(CS.PUB_DISABLE)))
                    {
                        await redis.KeyDeleteAsync(key);
                        continue;
                    }
                    string currentUserJson = await redis.StringGetAsync(key);
                    var currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserJson);
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
                        await redis.KeyDeleteAsync(key);
                        continue;
                    }
                    currentUser.SysUser = auth;
                    currentUser.Authorities = authorities;
                    currentUserJson = JsonConvert.SerializeObject(currentUser);
                    //����token  ʧЧʱ�䲻��
                    var cacheExpiry = await redis.KeyTimeToLiveAsync(key);
                    await redis.StringSetAsync(key, currentUserJson, cacheExpiry, When.Exists);
                }
            }
        }
    }
}