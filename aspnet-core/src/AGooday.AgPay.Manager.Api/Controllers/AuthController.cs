using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Manager.Api.Models;
using CaptchaGen.NetCore;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using AGooday.AgPay.Manager.Api.Extensions;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [ApiController]
    [Route("api/anon/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly ISysUserAuthService _sysUserAuthService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly IMemoryCache _cache;
        private readonly IDatabase _redis;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public AuthController(ILogger<AuthController> logger, IOptions<JwtSettings> jwtSettings, IMemoryCache cache, RedisUtil client,
            INotificationHandler<DomainNotification> notifications,
            ISysUserAuthService sysUserAuthService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _sysUserAuthService = sysUserAuthService;
            _cache = cache;
            _redis = client.GetDatabase();
            _notifications = (DomainNotificationHandler)notifications;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        [HttpPost]
        [Route("validate")]
        public ApiRes Validate(Validate model)
        {
            string account = Base64Util.DecodeBase64(model.ia);  //用户名 i account, 已做base64处理
            string ipassport = Base64Util.DecodeBase64(model.ip);    //密码 i passport,  已做base64处理
            string vercode = Base64Util.DecodeBase64(model.vc);  //验证码 vercode,  已做base64处理
            string vercodeToken = Base64Util.DecodeBase64(model.vt);	//验证码token, vercode token ,  已做base64处理

            string cacheCode = _redis.StringGet(CS.GetCacheKeyImgCode(vercodeToken));
            if (string.IsNullOrWhiteSpace(cacheCode) || !cacheCode.Equals(vercode))
            {
                throw new BizException("验证码有误！");
            }

            //登录方式， 默认为账号密码登录
            byte identityType = CS.AUTH_TYPE.LOGIN_USER_NAME;
            if (RegUtil.IsMobile(account))
            {
                identityType = CS.AUTH_TYPE.TELPHONE; //手机号登录
            }

            var auth = _sysUserAuthService.SelectByLogin(account, identityType, CS.SYS_TYPE.MGR);

            if (auth == null)
            {
                //没有该用户信息
                throw new BizException("用户名/密码错误！");
            }

            //https://jasonwatmore.com/post/2022/01/16/net-6-hash-and-verify-passwords-with-bcrypt
            //https://bcrypt.online/
            bool verified = BCrypt.Net.BCrypt.Verify(ipassport, auth.Credential);
            if (!verified)
            {
                //没有该用户信息
                throw new BizException("用户名/密码错误！");
            }

            //非超级管理员 && 不包含左侧菜单 进行错误提示
            if (auth.IsAdmin != CS.YES && !_sysRoleEntRelaService.UserHasLeftMenu(auth.SysUserId, auth.SysType))
            {
                throw new BizException("当前用户未分配任何菜单权限，请联系管理员进行分配后再登录！");
            }

            //生成token
            string cacheKey = CS.GetCacheKeyToken(auth.SysUserId, Guid.NewGuid().ToString("N").ToUpper());
            var authorities = _sysUserRoleRelaService.SelectRoleIdsByUserId(auth.SysUserId).ToList();
            authorities.AddRange(_sysRoleEntRelaService.SelectEntIdsByUserId(auth.SysUserId, auth.IsAdmin, auth.SysType));

            // 返回前端 accessToken
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, auth.Identifier),
                new Claim("userid",auth.SysUserId.ToString()),
                new Claim("avatar",auth.AvatarUrl),
                new Claim("displayName",auth.Realname),
                new Claim("loginName",auth.Realname),
                new Claim("telphone",auth.Telphone),
                new Claim("userNo",auth.UserNo.ToString()),
                new Claim("isAdmin",auth.IsAdmin.ToString()),
                new Claim("identityType",auth.IdentityType.ToString()),
                new Claim("sysType",auth.SysType),
                new Claim("cacheKey",cacheKey)
            });
            var accessToken = JwtBearerAuthenticationExtension.GetJwtAccessToken(_jwtSettings, claimsIdentity);

            var currentUser = JsonConvert.SerializeObject(new CurrentUser
            {
                CacheKey = cacheKey,
                User = auth,
                Authorities = authorities
            });
            _redis.StringSet(cacheKey, currentUser, new TimeSpan(0, 0, CS.TOKEN_TIME));

            // 删除图形验证码缓存数据
            _redis.KeyDelete(CS.GetCacheKeyImgCode(vercodeToken));

            return ApiRes.Ok4newJson(CS.ACCESS_TOKEN_NAME, accessToken);
        }

        [HttpGet]
        [Route("vercode")]
        public ApiRes Vercode()
        {
            //定义图形验证码的长和宽 // 4位验证码
            string code = ImageFactory.CreateCode(6);
            string imageBase64Data;
            using (var picStream = ImageFactory.BuildImage(code, 40, 137, 20, 10))
            {
                var imageBytes = picStream.ToArray();
                imageBase64Data = $"data:image/jpg;base64,{Convert.ToBase64String(imageBytes)}";
            }

            //redis
            string vercodeToken = Guid.NewGuid().ToString("N");
            _redis.StringSet(CS.GetCacheKeyImgCode(vercodeToken), code, new TimeSpan(0, 0, CS.VERCODE_CACHE_TIME)); //图片验证码缓存时间: 1分钟

            return ApiRes.Ok(new { imageBase64Data = imageBase64Data, vercodeToken = vercodeToken, expireTime = CS.VERCODE_CACHE_TIME });
        }
    }
}