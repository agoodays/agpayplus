using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using AGooday.AgPay.Merchant.Api.Models;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Merchant.Api.Attributes;

namespace AGooday.AgPay.Merchant.Api.Controllers
{
    [ApiController]
    [Route("api/current")]
    public class CurrentUserController : CommonController
    {
        private readonly ILogger<CurrentUserController> _logger;
        private readonly IDatabase _redis;
        private readonly ISysUserService _sysUserService;
        private readonly ISysEntitlementService _sysEntService;
        private readonly ISysUserAuthService _sysUserAuthService;
        private IMemoryCache _cache;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public CurrentUserController(ILogger<CurrentUserController> logger, IMemoryCache cache, INotificationHandler<DomainNotification> notifications, RedisUtil client,
            ISysUserService sysUserService,
            ISysEntitlementService sysEntService,
            ISysUserAuthService sysUserAuthService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysUserService = sysUserService;
            _sysEntService = sysEntService;
            _sysUserAuthService = sysUserAuthService;
            _cache = cache;
            _redis = client.GetDatabase();
            _notifications = (DomainNotificationHandler)notifications;
        }

        [HttpGet, Route("user"), NoLog]
        public ApiRes CurrentUserInfo()
        {
            try
            {
                //当前用户信息
                var currentUser = GetCurrentUser();
                if (currentUser == null)
                {
                    return ApiRes.CustomFail("登录失效");
                }

                //1. 当前用户所有权限ID集合
                var entIds = currentUser.Authorities.ToList();

                //2. 查询出用户所有菜单集合 (包含左侧显示菜单 和 其他类型菜单 )
                var sysEnts = _sysEntService.GetBySysType(CS.SYS_TYPE.MCH, entIds, new List<string> { CS.ENT_TYPE.MENU_LEFT, CS.ENT_TYPE.MENU_OTHER });

                //递归转换为树状结构
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var jsonArray = JArray.FromObject(sysEnts);
                var leftMenuTree = new TreeDataBuilder(jsonArray, "entId", "pid", "children", "entSort", true).BuildTreeObject();
                var user = JObject.FromObject(currentUser.SysUser);
                user.Add("entIdList", JArray.FromObject(entIds));
                user.Add("allMenuRouteTree", JToken.FromObject(leftMenuTree));
                return ApiRes.Ok(user);
            }
            catch (Exception)
            {
                return ApiRes.CustomFail("登录失效");
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
            _sysUserService.ModifyCurrentUserInfo(dto);
            var userinfo = _sysUserAuthService.GetUserAuthInfoById(currentUser.SysUser.SysUserId);
            currentUser.SysUser = userinfo;
            //保存redis最新数据
            var currentUserJson = JsonConvert.SerializeObject(currentUser);
            _redis.StringSet(currentUser.CacheKey, currentUserJson, new TimeSpan(0, 0, CS.TOKEN_TIME));
            return ApiRes.Ok();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPut, Route("modifyPwd"), MethodLog("修改密码")]
        public ApiRes ModifyPwd(ModifyPwd dto)
        {
            string currentUserPwd = Base64Util.DecodeBase64(dto.OriginalPwd); //当前用户登录密码
            var user = _sysUserAuthService.GetUserAuthInfoById(dto.SysUserId);
            bool verified = BCrypt.Net.BCrypt.Verify(currentUserPwd, user.Credential);
            //验证当前密码是否正确
            if (!verified)
            {
                throw new BizException("原密码验证失败！");
            }
            string opUserPwd = Base64Util.DecodeBase64(dto.ConfirmPwd);
            // 验证原密码与新密码是否相同
            if (opUserPwd.Equals(currentUserPwd))
            {
                throw new BizException("新密码与原密码不能相同！");
            }
            _sysUserAuthService.ResetAuthInfo(dto.SysUserId, null, null, opUserPwd, CS.SYS_TYPE.MCH);
            return Logout();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("logout"), MethodLog("退出")]
        public ApiRes Logout()
        {
            var currentUser = GetCurrentUser();
            _redis.KeyDelete(currentUser.CacheKey);
            return ApiRes.Ok();
        }
    }
}