using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Base.Api.Extensions;
using AGooday.AgPay.Base.Api.Models;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Agent.Api.Controllers
{
    [Route("api/current")]
    [ApiController]
    public class CurrentUserController : CommonController
    {
        private readonly IMemoryCache _cache;
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserAuthService _sysUserAuthService;
        private readonly ISysUserLoginAttemptService _sysUserLoginAttemptService;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public CurrentUserController(ILogger<CurrentUserController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMemoryCache cache,
            ISysUserService sysUserService,
            ISysUserAuthService sysUserAuthService,
            ISysUserLoginAttemptService sysUserLoginAttemptService,
            INotificationHandler<DomainNotification> notifications)
            : base(logger, cacheService, authService)
        {
            _cache = cache;
            _sysUserService = sysUserService;
            _sysUserAuthService = sysUserAuthService;
            _sysUserLoginAttemptService = sysUserLoginAttemptService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizeException"></exception>
        [HttpGet, Route("user"), NoLog]
        public async Task<ApiRes> CurrentUserInfoAsync()
        {
            try
            {
                //当前用户信息
                var currentUser = await GetCurrentUserAsync();
                var user = currentUser.SysUser;

                //1. 当前用户所有权限ID集合
                var entIds = currentUser.Authorities.ToList();

                //2. 查询出用户所有菜单集合 (包含左侧显示菜单 和 其他类型菜单 )
                var sysEnts = _authService.GetEntsBySysType(user.SysType, entIds, new List<string> { CS.ENT_TYPE.MENU_LEFT, CS.ENT_TYPE.MENU_OTHER });

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
                //1. 所有权限ID集合
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
            if (!await _cacheService.ExistsAsync(loginQRCacheKey))
            {
                throw new BizException("二维码无效，请刷新二维码后重新扫描");
            }
            var qrcodeInfo = await _cacheService.GetAsync<dynamic>(loginQRCacheKey);
            if (qrcodeInfo == null || qrcodeInfo.qrcodeStatus != CS.QR_CODE_STATUS.WAITING)
            {
                throw new BizException("二维码状态无效，请刷新二维码后重新扫描");
            }
            await _cacheService.UpdateWithExistingExpiryAsync(loginQRCacheKey, new { qrcodeStatus = CS.QR_CODE_STATUS.SCANNED });
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
            if (!await _cacheService.ExistsAsync(loginQRCacheKey))
            {
                throw new BizException("二维码无效，请刷新二维码后重新扫描");
            }
            var qrcodeInfo = await _cacheService.GetAsync<dynamic>(loginQRCacheKey);
            if (qrcodeInfo == null || qrcodeInfo.qrcodeStatus != CS.QR_CODE_STATUS.SCANNED)
            {
                throw new BizException("二维码状态无效，请刷新二维码后重新扫描");
            }
            if (isConfirm)
            {
                // 获取授权头的值
                var authorizationHeader = Request.Headers.Authorization;
                var accessToken = JwtBearerAuthenticationExtension.GetTokenFromAuthorizationHeader(authorizationHeader);
                var currentUser = await GetCurrentUserAsync();
                var lastLoginTime = await _sysUserLoginAttemptService.GetLastLoginTimeAsync(currentUser.SysUser.SysUserId);
                var qrcode = new Dictionary<string, object>();
                qrcode.Add(CS.ACCESS_TOKEN_NAME, accessToken);
                qrcode.Add("lastLoginTime", lastLoginTime);
                qrcode.Add("qrcodeStatus", CS.QR_CODE_STATUS.CONFIRMED);
                await _cacheService.UpdateWithExistingExpiryAsync(loginQRCacheKey, qrcode);
                return ApiRes.Ok();
            }
            else
            {
                await _cacheService.UpdateWithExistingExpiryAsync(loginQRCacheKey, new { qrcodeStatus = CS.QR_CODE_STATUS.CANCELED });
                return ApiRes.Ok();
            }
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("user"), MethodLog("修改个人信息")]
        public async Task<ApiRes> ModifyCurrentUserInfoAsync(ModifyCurrentUserInfoDto dto)
        {
            var currentUser = await GetCurrentUserAsync();
            dto.SysUserId = currentUser.SysUser.SysUserId;
            await _sysUserService.ModifyCurrentUserInfoAsync(dto);
            var userinfo = await _authService.GetUserAuthInfoByIdAsync(currentUser.SysUser.SysUserId);
            currentUser.SysUser = userinfo;
            //保存redis最新数据
            await _cacheService.SetAsync(currentUser.CacheKey, currentUser, new TimeSpan(0, 0, CS.TOKEN_TIME));
            return ApiRes.Ok();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPut, Route("modifyPwd"), MethodLog("修改密码")]
        public async Task<ApiRes> ModifyPwdAsync(ModifyPwd model)
        {
            var currentUser = await GetCurrentUserAsync();
            string currentUserPwd = Base64Util.DecodeBase64(model.OriginalPwd); //当前用户登录密码
            var user = await _authService.GetUserAuthInfoByIdAsync(currentUser.SysUser.SysUserId);
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
            await _sysUserAuthService.ResetAuthInfoAsync(user.SysUserId, null, null, opUserPwd, user.SysType);
            return await LogoutAsync();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("logout"), MethodLog("退出登录")]
        public async Task<ApiRes> LogoutAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            await _cacheService.RemoveAsync(currentUser.CacheKey);
            return ApiRes.Ok();
        }
    }
}