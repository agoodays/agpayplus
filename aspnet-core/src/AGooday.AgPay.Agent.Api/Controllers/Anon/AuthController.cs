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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Agent.Api.Controllers.Anon
{
    /// <summary>
    /// 认证接口
    /// </summary>
    [ApiController, AllowAnonymous]
    [Route("api/anon")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserAuthService _sysUserAuthService;
        private readonly ISysUserLoginAttemptService _sysUserLoginAttemptService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysConfigService _sysConfigService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly ISysLogService _sysLogService;
        private readonly IMemoryCache _cache;
        private readonly IDatabase _redis;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        private const string AUTH_METHOD_REMARK = "登录认证"; //用户信息认证方法描述

        public AuthController(ILogger<AuthController> logger, IOptions<JwtSettings> jwtSettings, IMemoryCache cache, RedisUtil client,
            INotificationHandler<DomainNotification> notifications,
            ISysUserService sysUserService,
            ISysUserAuthService sysUserAuthService,
            ISysUserLoginAttemptService sysUserLoginAttemptService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService,
            ISysConfigService sysConfigService,
            IAgentInfoService agentInfoService,
            ISysLogService sysLogService)
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _sysUserService = sysUserService;
            _sysUserAuthService = sysUserAuthService;
            _sysUserLoginAttemptService = sysUserLoginAttemptService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysConfigService = sysConfigService;
            _agentInfoService = agentInfoService;
            _sysLogService = sysLogService;
            _cache = cache;
            _redis = client.GetDatabase();
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 用户信息认证 获取iToken
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("auth/validate"), MethodLog(AUTH_METHOD_REMARK)]
        public async Task<ApiRes> Validate(Validate model)
        {
            string account = Base64Util.DecodeBase64(model.ia); //用户名 i account, 已做base64处理
            string ipassport = Base64Util.DecodeBase64(model.ip); //密码 i passport, 已做base64处理
            string vercode = Base64Util.DecodeBase64(model.vc); //验证码 vercode, 已做base64处理
            string vercodeToken = Base64Util.DecodeBase64(model.vt); //验证码token, vercode token , 已做base64处理
            string codeCacheKey = CS.GetCacheKeyImgCode(vercodeToken);
#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode) || !cacheCode.Equals(vercode))
            {
                throw new BizException("验证码有误！");
            } 
#endif

            //登录方式， 默认为账号密码登录
            byte identityType = CS.AUTH_TYPE.LOGIN_USER_NAME;
            if (RegUtil.IsMobile(account))
            {
                identityType = CS.AUTH_TYPE.TELPHONE; //手机号登录
            }

            var auth = _sysUserAuthService.SelectByLogin(account, identityType, CS.SYS_TYPE.AGENT);

            if (auth == null)
            {
                //没有该用户信息
                throw new BizException("用户名/密码错误！");
            }

            int maxLoginAttempts = 5;
            int failedAttempts = 0;
            DateTime? lastLoginTime = null;
            var loginErrorMessage = "密码输入错误次数超限，请稍后再试！";
            if (maxLoginAttempts > 0)
            {
                (failedAttempts, lastLoginTime) = await _sysUserLoginAttemptService.GetFailedLoginAttemptsAsync(auth.SysUserId, TimeSpan.FromMinutes(15));
                if (failedAttempts >= maxLoginAttempts)
                {
                    throw new BizException(loginErrorMessage);
                }
            }

            //https://jasonwatmore.com/post/2022/01/16/net-6-hash-and-verify-passwords-with-bcrypt
            //https://bcrypt.online/
            bool verified = BCryptUtil.VerifyHash(ipassport, auth.Credential);
            var loginAttempt = new SysUserLoginAttemptDto()
            {
                UserId = auth.SysUserId,
                IdentityType = auth.IdentityType,
                Identifier = auth.Identifier,
                IpAddress = string.Empty,
                SysType = CS.SYS_TYPE.AGENT,
                AttemptTime = DateTime.Now,
                Success = false
            };
            if (!verified)
            {
                await _sysUserLoginAttemptService.RecordLoginAttemptAsync(loginAttempt);
                ++failedAttempts;
                loginErrorMessage = failedAttempts >= maxLoginAttempts ? loginErrorMessage : $"用户名/密码错误，还可尝试{maxLoginAttempts - failedAttempts}次，失败将锁定15分钟！";
                //没有该用户信息
                throw new BizException(loginErrorMessage);
            }
            loginAttempt.Success = true;
            await _sysUserLoginAttemptService.RecordLoginAttemptAsync(loginAttempt);
            // 登录成功，清除登录尝试记录
            await _sysUserLoginAttemptService.ClearFailedLoginAttemptsAsync(auth.SysUserId);
            return Auth(auth, codeCacheKey, lastLoginTime);
        }

        private ApiRes Auth(SysUserAuthInfoDto auth, string codeCacheKey, DateTime? lastLoginTime = null)
        {
            //非超级管理员 && 不包含左侧菜单 进行错误提示
            if (auth.IsAdmin != CS.YES && !_sysRoleEntRelaService.UserHasLeftMenu(auth.SysUserId, auth.SysType))
            {
                throw new BizException("当前用户未分配任何菜单权限，请联系管理员进行分配后再登录！");
            }

            var agent = _agentInfoService.GetById(auth.BelongInfoId);
            auth.ShortName = agent.AgentShortName;

            //生成token
            string cacheKey = CS.GetCacheKeyToken(auth.SysUserId, Guid.NewGuid().ToString("N").ToUpper());
            var authorities = _sysUserRoleRelaService.SelectRoleIdsByUserId(auth.SysUserId).ToList();
            authorities.AddRange(_sysRoleEntRelaService.SelectEntIdsByUserId(auth.SysUserId, auth.UserType, auth.SysType));

            // 返回前端 accessToken
            TokenModelJwt tokenModel = new TokenModelJwt();
            tokenModel.SysUserId = auth.SysUserId.ToString();
            tokenModel.AvatarUrl = auth.AvatarUrl;
            tokenModel.Realname = auth.Realname;
            tokenModel.LoginUsername = auth.LoginUsername;
            tokenModel.Telphone = auth.Telphone;
            tokenModel.UserNo = auth.UserNo.ToString();
            tokenModel.Sex = auth.Sex.ToString();
            tokenModel.State = auth.State.ToString();
            tokenModel.IsAdmin = auth.IsAdmin.ToString();
            tokenModel.SysType = auth.SysType;
            tokenModel.BelongInfoId = auth.BelongInfoId;
            tokenModel.CreatedAt = auth.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            tokenModel.UpdatedAt = auth.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            tokenModel.CacheKey = cacheKey;
            var accessToken = JwtBearerAuthenticationExtension.IssueJwt(_jwtSettings, tokenModel);

            var currentUser = JsonConvert.SerializeObject(new CurrentUser
            {
                CacheKey = cacheKey,
                SysUser = auth,
                Authorities = authorities
            });
            _redis.StringSet(cacheKey, currentUser, new TimeSpan(0, 0, CS.TOKEN_TIME));

            // 删除验证码缓存数据
            _redis.KeyDelete(codeCacheKey);

            if (lastLoginTime != null)
            {
                var data = new Dictionary<string, object>();
                data.Add(CS.ACCESS_TOKEN_NAME, accessToken);
                data.Add("lastLoginTime", lastLoginTime);
                return ApiRes.Ok(data);
            }
            return ApiRes.Ok4newJson(CS.ACCESS_TOKEN_NAME, accessToken);
        }

        /// <summary>
        /// 用户信息认证 获取iToken
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("auth/phoneCode"), MethodLog(AUTH_METHOD_REMARK)]
        public async Task<ApiRes> PhoneCode(PhoneCode model)
        {
            string phone = Base64Util.DecodeBase64(model.phone);
            string code = Base64Util.DecodeBase64(model.code);
            string smsCodeToken = $"{CS.SYS_TYPE.AGENT.ToLower()}_{CS.SMS_TYPE.AUTH}_{phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);
#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode))
            {
                throw new BizException("验证码已过期，请重新点击发送验证码！");
            }
            if (!cacheCode.Equals(code))
            {
                throw new BizException("验证码有误！");
            }
#endif
            byte identityType = CS.AUTH_TYPE.TELPHONE;
            var auth = _sysUserAuthService.SelectByLogin(phone, identityType, CS.SYS_TYPE.AGENT);

            if (auth == null)
            {
                //没有该用户信息
                throw new BizException("未绑定手机号！");
            }
            (int failedAttempts, DateTime? lastLoginTime) = await _sysUserLoginAttemptService.GetFailedLoginAttemptsAsync(auth.SysUserId, TimeSpan.FromMinutes(15));
            return Auth(auth, codeCacheKey, lastLoginTime);
        }

        /// <summary>
        /// 图片验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("auth/vercode"), NoLog]
        public ApiRes Vercode()
        {
            //定义图形验证码的长和宽 // 6位验证码
            //string code = ImageFactory.CreateCode(6);
            //string imageBase64Data;
            //using (var picStream = ImageFactory.BuildImage(code, 40, 137, 20, 10))
            //{
            //    var imageBytes = picStream.ToArray();
            //    imageBase64Data = $"data:image/jpg;base64,{Convert.ToBase64String(imageBytes)}";
            //}
            var code = VerificationCodeUtil.RandomVerificationCode(6);
            var bitmap = VerificationCodeUtil.DrawImage(code, 137, 40, 20);
            var imageBase64Data = $"data:image/jpg;base64,{VerificationCodeUtil.BitmapToBase64Str(bitmap)}";

            //redis
            string vercodeToken = Guid.NewGuid().ToString("N");
            string codeCacheKey = CS.GetCacheKeyImgCode(vercodeToken);
            _redis.StringSet(codeCacheKey, code, new TimeSpan(0, 0, CS.VERCODE_CACHE_TIME)); //图片验证码缓存时间: 1分钟

            return ApiRes.Ok(new { imageBase64Data, vercodeToken, expireTime = CS.VERCODE_CACHE_TIME });
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("register/agentRegister"), MethodLog("代理商注册")]
        public ApiRes Register(Register model)
        {
            string phone = Base64Util.DecodeBase64(model.phone);
            string code = Base64Util.DecodeBase64(model.code);
            string confirmPwd = Base64Util.DecodeBase64(model.confirmPwd);
            string smsCodeToken = $"{CS.SYS_TYPE.AGENT.ToLower()}_{CS.SMS_TYPE.REGISTER}_{phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);

#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode))
            {
                throw new BizException("验证码已过期，请重新点击发送验证码！");
            }
            if (!cacheCode.Equals(code))
            {
                throw new BizException("验证码有误！");
            }
#endif

            // 删除短信验证码缓存数据
            _redis.KeyDelete(codeCacheKey);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 获取站点信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("siteInfos"), NoLog]
        public ApiRes SiteInfos()
        {
            var configList = _sysConfigService.GetKeyValueByGroupKey("oemConfig", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 获取条约
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("treaty"), NoLog]
        public ApiRes Treaty()
        {
            var configList = _sysConfigService.GetKeyValueByGroupKey("agentTreatyConfig", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("sms/code"), NoLog]
        public ApiRes SendCode(SmsCode model)
        {
            if (model.smsType.Equals(CS.SMS_TYPE.REGISTER) && _sysUserService.IsExistTelphone(model.phone, CS.SYS_TYPE.AGENT))
            {
                throw new BizException("当前用户已存在！");
            }

            if (model.smsType.Equals(CS.SMS_TYPE.RETRIEVE) && !_sysUserService.IsExistTelphone(model.phone, CS.SYS_TYPE.AGENT))
            {
                throw new BizException("用户不存在！");
            }

            if (model.smsType.Equals(CS.SMS_TYPE.AUTH) && !_sysUserService.IsExistTelphone(model.phone, CS.SYS_TYPE.AGENT))
            {
                throw new BizException("用户不存在！");
            }

            var code = VerificationCodeUtil.RandomVerificationCode(6);

            //redis
            string smsCodeToken = $"{CS.SYS_TYPE.AGENT.ToLower()}_{model.smsType}_{model.phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);
            _redis.StringSet(codeCacheKey, code, new TimeSpan(0, 0, CS.SMSCODE_CACHE_TIME)); //短信验证码缓存时间: 1分钟

            return ApiRes.Ok();
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("cipher/retrieve"), MethodLog("密码找回")]
        public ApiRes Retrieve(Retrieve model)
        {
            string phone = Base64Util.DecodeBase64(model.phone);
            string code = Base64Util.DecodeBase64(model.code);
            string newPwd = Base64Util.DecodeBase64(model.newPwd);
            string smsCodeToken = $"{CS.SYS_TYPE.AGENT.ToLower()}_{CS.SMS_TYPE.RETRIEVE}_{phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);

#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode))
            {
                throw new BizException("验证码已过期，请重新点击发送验证码！");
            }
            if (!cacheCode.Equals(code))
            {
                throw new BizException("验证码有误！");
            }
#endif
            var sysUser = _sysUserService.GetByTelphone(model.phone, CS.SYS_TYPE.AGENT);
            if (sysUser == null)
            {
                throw new BizException("用户不存在！");
            }
            if (sysUser.State.Equals(CS.PUB_DISABLE))
            {
                throw new BizException("用户已停用！");
            }
            var sysUserAuth = _sysUserAuthService.GetByIdentifier(CS.AUTH_TYPE.TELPHONE, model.phone, CS.SYS_TYPE.AGENT);
            if (sysUserAuth == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            bool verified = BCryptUtil.VerifyHash(newPwd, sysUserAuth.Credential);
            if (verified)
            {
                throw new BizException("新密码与原密码相同！");
            }
            _sysUserAuthService.ResetAuthInfo(sysUser.SysUserId.Value, null, null, newPwd, CS.SYS_TYPE.AGENT);
            // 删除短信验证码缓存数据
            _redis.KeyDelete(codeCacheKey);
            return ApiRes.Ok();
        }
    }
}