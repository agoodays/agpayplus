using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Extensions;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace AGooday.AgPay.Agent.Api.Controllers
{
    [Route("api/current")]
    [ApiController]
    public class CurrentUserController : CommonController
    {
        private readonly IDatabase _redis;
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserAuthService _sysUserAuthService;
        private readonly ISysUserLoginAttemptService _sysUserLoginAttemptService;
        private readonly IMemoryCache _cache;
        private readonly IAuthService _authService;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public CurrentUserController(ILogger<CurrentUserController> logger,
            IMemoryCache cache,
            ISysUserService sysUserService,
            ISysUserAuthService sysUserAuthService,
            ISysUserLoginAttemptService sysUserLoginAttemptService,
            INotificationHandler<DomainNotification> notifications,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysUserService = sysUserService;
            _sysUserAuthService = sysUserAuthService;
            _sysUserLoginAttemptService = sysUserLoginAttemptService;
            _cache = cache;
            _redis = client.GetDatabase();
            _authService = authService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizeException"></exception>
        [HttpGet, Route("user"), NoLog]
        public ApiRes CurrentUserInfo()
        {
            try
            {
                //当前用户信息
                var currentUser = GetCurrentUser();
                var user = currentUser.SysUser;

                //1. 当前用户所有权限ID集合
                var entIds = currentUser.Authorities.ToList();

                //2. 查询出用户所有菜单集合 (包含左侧显示菜单 和 其他类型菜单 )
                var sysEnts = _authService.GetEntsBySysType(CS.SYS_TYPE.AGENT, entIds, new List<string> { CS.ENT_TYPE.MENU_LEFT, CS.ENT_TYPE.MENU_OTHER });

                //递归转换为树状结构
                //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                //{
                //    Formatting = Formatting.Indented,
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //};
                var jsonArray = JArray.FromObject(sysEnts);
                var allMenuRouteTree = new TreeDataBuilder(jsonArray, "entId", "pid", "children", "entSort", true).BuildTreeObject();
                //var user = JObject.FromObject(currentUser.SysUser);
                //user.Add("entIdList", JArray.FromObject(entIds));
                //user.Add("allMenuRouteTree", JToken.FromObject(allMenuRouteTree));
                user.AddExt("entIdList", entIds);
                user.AddExt("allMenuRouteTree", allMenuRouteTree);
                return ApiRes.Ok(user);
            }
            catch (Exception)
            {
                throw new UnauthorizeException();
                //throw new BizException("登录失效");
                //return ApiRes.CustomFail("登录失效");
            }
        }

        /// <summary>
        /// 当前用户扫码
        /// </summary>
        /// <param name="qrcodeNo"></param>
        /// <returns></returns>
        [HttpGet, Route("user/scan"), NoLog]
        public async Task<ApiRes> ScanAsync(string qrcodeNo)
        {
            string loginQRCacheKey = CS.GetCacheKeyLoginQR(qrcodeNo);
            if (!_redis.KeyExists(loginQRCacheKey))
            {
                throw new BizException("二维码无效，请刷新二维码后重新扫描");
            }
            string data = await _redis.StringGetAsync(loginQRCacheKey);
            var qrcodeInfo = JsonConvert.DeserializeObject<dynamic>(string.IsNullOrWhiteSpace(data) ? "{}" : data);
            if (string.IsNullOrWhiteSpace(data) || qrcodeInfo.qrcodeStatus != CS.QR_CODE_STATUS.WAITING)
            {
                throw new BizException("二维码状态无效，请刷新二维码后重新扫描");
            }
            var cacheExpiry = _redis.KeyTimeToLive(loginQRCacheKey);
            await _redis.StringSetAsync(loginQRCacheKey, JsonConvert.SerializeObject(new { qrcodeStatus = CS.QR_CODE_STATUS.SCANNED }), cacheExpiry, When.Exists);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 当前用户确认登录
        /// </summary>
        /// <param name="qrcodeNo"></param>
        /// <param name="isConfirm"></param>
        /// <returns></returns>
        [HttpGet, Route("user/confirmLogin"), NoLog]
        public async Task<ApiRes> ConfirmLoginAsync(string qrcodeNo, bool isConfirm = true)
        {
            string loginQRCacheKey = CS.GetCacheKeyLoginQR(qrcodeNo);
            if (!_redis.KeyExists(loginQRCacheKey))
            {
                throw new BizException("二维码无效，请刷新二维码后重新扫描");
            }
            string qrcodeData = await _redis.StringGetAsync(loginQRCacheKey);
            var qrcodeInfo = JsonConvert.DeserializeObject<dynamic>(string.IsNullOrWhiteSpace(qrcodeData) ? "{}" : qrcodeData);
            if (string.IsNullOrWhiteSpace(qrcodeData) || qrcodeInfo.qrcodeStatus != CS.QR_CODE_STATUS.SCANNED)
            {
                throw new BizException("二维码状态无效，请刷新二维码后重新扫描");
            }
            var cacheExpiry = await _redis.KeyTimeToLiveAsync(loginQRCacheKey);
            if (isConfirm)
            {
                // 获取授权头的值
                var authorizationHeader = Request.Headers.Authorization;
                var accessToken = JwtBearerAuthenticationExtension.GetTokenFromAuthorizationHeader(authorizationHeader);
                var currentUser = GetCurrentUser();
                var lastLoginTime = await _sysUserLoginAttemptService.GetLastLoginTimeAsync(currentUser.SysUser.SysUserId);
                var qrcode = new Dictionary<string, object>();
                qrcode.Add(CS.ACCESS_TOKEN_NAME, accessToken);
                qrcode.Add("lastLoginTime", lastLoginTime);
                qrcode.Add("qrcodeStatus", CS.QR_CODE_STATUS.CONFIRMED);
                await _redis.StringSetAsync(loginQRCacheKey, JsonConvert.SerializeObject(qrcode), cacheExpiry);
                return ApiRes.Ok();
            }
            else
            {
                await _redis.StringSetAsync(loginQRCacheKey, JsonConvert.SerializeObject(new { qrcodeStatus = CS.QR_CODE_STATUS.CANCELED }), cacheExpiry);
                return ApiRes.Ok();
            }
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("user"), MethodLog("修改个人信息")]
        public ApiRes ModifyCurrentUserInfo(ModifyCurrentUserInfoDto dto)
        {
            var currentUser = GetCurrentUser();
            dto.SysUserId = currentUser.SysUser.SysUserId;
            _sysUserService.ModifyCurrentUserInfo(dto);
            var userinfo = _authService.GetUserAuthInfoById(currentUser.SysUser.SysUserId);
            currentUser.SysUser = userinfo;
            //保存redis最新数据
            var currentUserJson = JsonConvert.SerializeObject(currentUser);
            _redis.StringSet(currentUser.CacheKey, currentUserJson, new TimeSpan(0, 0, CS.TOKEN_TIME));
            return ApiRes.Ok();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPut, Route("modifyPwd"), MethodLog("修改密码")]
        public ApiRes ModifyPwd(ModifyPwd model)
        {
            var currentUser = GetCurrentUser();
            string currentUserPwd = Base64Util.DecodeBase64(model.OriginalPwd); //当前用户登录密码currentUser
            var user = _authService.GetUserAuthInfoById(currentUser.SysUser.SysUserId);
            bool verified = BCryptUtil.VerifyHash(currentUserPwd, user.Credential);
            //验证当前密码是否正确
            if (!verified)
            {
                throw new BizException("原密码验证失败！");
            }
            string opUserPwd = Base64Util.DecodeBase64(model.ConfirmPwd);
            // 验证原密码与新密码是否相同
            if (opUserPwd.Equals(currentUserPwd))
            {
                throw new BizException("新密码与原密码不能相同！");
            }
            _sysUserAuthService.ResetAuthInfo(user.SysUserId, null, null, opUserPwd, CS.SYS_TYPE.AGENT);
            return Logout();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("logout"), MethodLog("退出登录")]
        public ApiRes Logout()
        {
            var currentUser = GetCurrentUser();
            _redis.KeyDelete(currentUser.CacheKey);
            return ApiRes.Ok();
        }
    }
}