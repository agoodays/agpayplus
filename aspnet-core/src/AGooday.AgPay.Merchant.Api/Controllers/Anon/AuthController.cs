using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.SMS.Extensions;
using AGooday.AgPay.Components.SMS.Models;
using AGooday.AgPay.Components.SMS.Services;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Extensions;
using AGooday.AgPay.Merchant.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Merchant.Api.Controllers.Anon
{
    /// <summary>
    /// ��֤�ӿ�
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
        private readonly ISysConfigService _sysConfigService;
        private readonly ISmsService smsService;
        private readonly IMemoryCache _cache;
        private readonly IDatabase _redis;
        private readonly IAuthService _authService;
        // ������֪ͨ�������ע��Controller
        private readonly DomainNotificationHandler _notifications;

        private const string AUTH_METHOD_REMARK = "��¼��֤"; //�û���Ϣ��֤��������

        public AuthController(ILogger<AuthController> logger, IOptions<JwtSettings> jwtSettings, IMemoryCache cache, RedisUtil client,
            INotificationHandler<DomainNotification> notifications,
            ISysUserService sysUserService,
            ISysUserAuthService sysUserAuthService,
            ISysUserLoginAttemptService sysUserLoginAttemptService,
            ISysConfigService sysConfigService,
            ISmsServiceFactory smsServiceFactory,
            IAuthService authService)
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _sysUserService = sysUserService;
            _sysUserAuthService = sysUserAuthService;
            _sysUserLoginAttemptService = sysUserLoginAttemptService;
            _sysConfigService = sysConfigService;
            this.smsService = smsServiceFactory.GetService();
            _cache = cache;
            _redis = client.GetDatabase();
            _authService = authService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// �û���Ϣ��֤ ��ȡiToken
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("auth/validate"), MethodLog(AUTH_METHOD_REMARK)]
        public async Task<ApiRes> ValidateAsync(Validate model)
        {
            string account = Base64Util.DecodeBase64(model.ia); //�û��� i account, ����base64����
            string ipassport = Base64Util.DecodeBase64(model.ip); //���� i passport, ����base64����
            string vercode = Base64Util.DecodeBase64(model.vc); //��֤�� vercode, ����base64����
            string vercodeToken = Base64Util.DecodeBase64(model.vt); //��֤��token, vercode token , ����base64����
            string codeCacheKey = CS.GetCacheKeyImgCode(vercodeToken);
#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode) || !cacheCode.Equals(vercode, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException("��֤������");
            }
#endif

            //��¼��ʽ�� Ĭ��Ϊ�˺������¼
            byte identityType = CS.AUTH_TYPE.LOGIN_USER_NAME;
            if (RegUtil.IsMobile(account))
            {
                identityType = CS.AUTH_TYPE.TELPHONE; //�ֻ��ŵ�¼
            }

            var auth = _authService.LoginAuth(account, identityType, CS.SYS_TYPE.MCH);

            if (auth == null)
            {
                //û�и��û���Ϣ
                throw new BizException("�û���/�������");
            }

            var sysConfig = _sysConfigService.GetByKey("loginErrorMaxLimit", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            var loginErrorMaxLimit = JsonConvert.DeserializeObject<Dictionary<string, int>>(sysConfig.ConfigVal);
            loginErrorMaxLimit.TryGetValue("limitMinute", out int limitMinute);
            loginErrorMaxLimit.TryGetValue("maxLoginAttempts", out int maxLoginAttempts);
            var loginErrorMessage = "�����������������ޣ����Ժ����ԣ�";
            (int failedAttempts, DateTime? lastLoginTime) = await _sysUserLoginAttemptService.GetFailedLoginAttemptsAsync(auth.SysUserId, TimeSpan.FromMinutes(limitMinute));
            if (failedAttempts >= maxLoginAttempts && maxLoginAttempts > 0)
            {
                throw new BizException(loginErrorMessage);
            }

            //https://jasonwatmore.com/post/2022/01/16/net-6-hash-and-verify-passwords-with-bcrypt
            //https://bcrypt.online/
            bool verified = BCryptUtil.VerifyHash(ipassport, auth.Credential);
            var loginAttempt = new SysUserLoginAttemptDto()
            {
                UserId = auth.SysUserId,
                IdentityType = auth.IdentityType,
                Identifier = auth.Identifier,
                IpAddress = IpUtil.GetIP(Request),
                SysType = CS.SYS_TYPE.MCH,
                AttemptTime = DateTime.Now,
                Success = false
            };
            if (!verified)
            {
                await _sysUserLoginAttemptService.RecordLoginAttemptAsync(loginAttempt);
                ++failedAttempts;
                loginErrorMessage = maxLoginAttempts > 0 ? failedAttempts >= maxLoginAttempts ? loginErrorMessage : $"�û���/������󣬻��ɳ���{maxLoginAttempts - failedAttempts}�Σ�ʧ�ܽ�����{limitMinute}���ӣ�" : "�û���/�������";
                //û�и��û���Ϣ
                throw new BizException(loginErrorMessage);
            }
            loginAttempt.Success = true;
            await _sysUserLoginAttemptService.RecordLoginAttemptAsync(loginAttempt);
            // ��¼�ɹ��������¼���Լ�¼
            await _sysUserLoginAttemptService.ClearFailedLoginAttemptsAsync(auth.SysUserId);
            return Auth(auth, codeCacheKey, lastLoginTime);
        }

        private ApiRes Auth(SysUserAuthInfoDto auth, string codeCacheKey, DateTime? lastLoginTime = null)
        {
            //�ǳ�������Ա && ���������˵� ���д�����ʾ
            if (auth.IsAdmin != CS.YES && !_authService.UserHasLeftMenu(auth.SysUserId, auth.SysType))
            {
                if (auth.UserType.Equals(CS.USER_TYPE.OPERATOR))
                {
                    throw new BizException("��ǰ�û�δ�����κβ˵�Ȩ�ޣ�����ϵ����Ա���з�����ٵ�¼��");
                }
            }

            auth.GetEnts(_authService, out List<string> authorities, out List<SysEntitlementDto> ents);

            if (ents.Count <= 0)
            {
                throw new BizException("��ǰ�û�δ�����κβ˵�Ȩ�ޣ�����ϵ����Ա���з�����ٵ�¼��");
            }

            authorities.AddRange(ents.Select(s => s.EntId));

            //����token
            string cacheKey = CS.GetCacheKeyToken(auth.SysUserId, Guid.NewGuid().ToString("N").ToUpper());

            // ����ǰ�� accessToken
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

            // ɾ����֤�뻺������
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
        /// �û���Ϣ��֤ ��ȡiToken
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("auth/phoneCode"), MethodLog(AUTH_METHOD_REMARK)]
        public async Task<ApiRes> PhoneCodeAsync(PhoneCode model)
        {
            string phone = Base64Util.DecodeBase64(model.phone);
            string code = Base64Util.DecodeBase64(model.code);
            string smsCodeToken = $"{CS.SYS_TYPE.MCH.ToLower()}_{CS.SMS_TYPE.AUTH}_{phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);
#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode))
            {
                throw new BizException("��֤���ѹ��ڣ������µ��������֤�룡");
            }
            if (!cacheCode.Equals(code))
            {
                throw new BizException("��֤������");
            }
#endif
            byte identityType = CS.AUTH_TYPE.TELPHONE;
            var auth = _authService.LoginAuth(phone, identityType, CS.SYS_TYPE.MCH);

            if (auth == null)
            {
                //û�и��û���Ϣ
                throw new BizException("δ���ֻ��ţ�");
            }
            (int failedAttempts, DateTime? lastLoginTime) = await _sysUserLoginAttemptService.GetFailedLoginAttemptsAsync(auth.SysUserId, TimeSpan.FromMinutes(15));
            return Auth(auth, codeCacheKey, lastLoginTime);
        }

        /// <summary>
        /// ��ȡ��ά�����ݻ��ȡ��ά��״̬ 
        /// ��ά��״̬��waiting-��ɨ�裬scanned-��ɨ�裬expired-�ѹ��ڣ�confirmed-��ȷ�ϣ�canceled-��ȡ��
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("auth/qrcodeStatus"), NoLog]
        public ApiRes QrCodeStatus(string qrcodeNo)
        {
            if (string.IsNullOrWhiteSpace(qrcodeNo))
            {
                qrcodeNo = CS.LOGIN_QR_CODE_NO;
                string loginQRCacheKey = CS.GetCacheKeyLoginQR(qrcodeNo);
                _redis.StringSet(loginQRCacheKey, JsonConvert.SerializeObject(new { qrcodeStatus = CS.QR_CODE_STATUS.WAITING }), new TimeSpan(0, 0, CS.LOGIN_QR_CACHE_TIME)); //��¼��ά�뻺��ʱ��: 1����
                return ApiRes.Ok(new { qrcodeNo });
            }
            else
            {
                string loginQRCacheKey = CS.GetCacheKeyLoginQR(qrcodeNo);
                string qrcodeStatus = _redis.StringGet(loginQRCacheKey);
                return ApiRes.Ok(string.IsNullOrWhiteSpace(qrcodeStatus) ? new { qrcodeStatus = CS.QR_CODE_STATUS.EXPIRED } : JsonConvert.DeserializeObject(qrcodeStatus));
            }
        }

        /// <summary>
        /// ͼƬ��֤��
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("auth/vercode"), NoLog]
        public ApiRes Vercode()
        {
            //����ͼ����֤��ĳ��Ϳ� // 4λ��֤��
            //string code = ImageFactory.CreateCode(6);
            //string imageBase64Data;
            //using (var picStream = ImageFactory.BuildImage(code, 40, 137, 20, 10))
            //{
            //    var imageBytes = picStream.ToArray();
            //    imageBase64Data = $"data:image/jpg;base64,{Convert.ToBase64String(imageBytes)}";
            //}
            var code = VerificationCodeUtil.RandomVerificationCode(4);
            var bitmap = VerificationCodeUtil.DrawImage(code, 137, 40, 20);
            //var imageBase64Data = $"data:image/jpg;base64,{VerificationCodeUtil.BitmapToBase64String(bitmap)}";
            var imageBase64Data = VerificationCodeUtil.BitmapToImageBase64String(bitmap);

            //redis
            string vercodeToken = Guid.NewGuid().ToString("N");
            string codeCacheKey = CS.GetCacheKeyImgCode(vercodeToken);
            _redis.StringSet(codeCacheKey, code, new TimeSpan(0, 0, CS.VERCODE_CACHE_TIME)); //ͼƬ��֤�뻺��ʱ��: 1����

            return ApiRes.Ok(new { imageBase64Data, vercodeToken, expireTime = CS.VERCODE_CACHE_TIME });
        }

        /// <summary>
        /// ע��
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("register/mchRegister"), MethodLog("�̻�ע��")]
        public ApiRes Register(Register model)
        {
            string phone = Base64Util.DecodeBase64(model.phone);
            string code = Base64Util.DecodeBase64(model.code);
            string confirmPwd = Base64Util.DecodeBase64(model.confirmPwd);
            string smsCodeToken = $"{CS.SYS_TYPE.MCH.ToLower()}_{CS.SMS_TYPE.REGISTER}_{phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);

#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode))
            {
                throw new BizException("��֤���ѹ��ڣ������µ��������֤�룡");
            }
            if (!cacheCode.Equals(code))
            {
                throw new BizException("��֤������");
            }
#endif

            // ɾ��������֤�뻺������
            _redis.KeyDelete(codeCacheKey);
            return ApiRes.Ok();
        }

        /// <summary>
        /// ��ȡվ����Ϣ
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("siteInfos"), NoLog]
        public ApiRes SiteInfos()
        {
            var configList = _sysConfigService.GetKeyValueByGroupKey("oemConfig", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// ��ȡ��Լ
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("treaty"), NoLog]
        public ApiRes Treaty()
        {
            var configList = _sysConfigService.GetKeyValueByGroupKey("mchTreatyConfig", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// ���Ͷ�����֤��
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("sms/code"), NoLog]
        public ApiRes SendCode(SmsCode model)
        {
            if (model.smsType.Equals(CS.SMS_TYPE.REGISTER) && _sysUserService.IsExistTelphone(model.phone, CS.SYS_TYPE.MCH))
            {
                throw new BizException("��ǰ�û��Ѵ��ڣ�");
            }

            if ((model.smsType.Equals(CS.SMS_TYPE.RETRIEVE) || model.smsType.Equals(CS.SMS_TYPE.AUTH))
                && !_sysUserService.IsExistTelphone(model.phone, CS.SYS_TYPE.MCH))
            {
                throw new BizException("�û������ڣ�");
            }

            var code = SmsVerificationCodeGenerator.GenerateCode(4);

            //redis
            string smsCodeToken = $"{CS.SYS_TYPE.MCH.ToLower()}_{model.smsType}_{model.phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);
            _redis.StringSet(codeCacheKey, code, new TimeSpan(0, 0, CS.SMSCODE_CACHE_TIME)); //������֤�뻺��ʱ��: 1����
#if !DEBUG
            smsService.SendVercode(new SmsBizVercodeModel()
            {
                Mobile = model.phone,
                Vercode = code,
                SmsType = model.smsType
            }); 
#endif
            return ApiRes.Ok();
        }

        /// <summary>
        /// �һ�����
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("cipher/retrieve"), MethodLog("�����һ�")]
        public ApiRes Retrieve(Retrieve model)
        {
            string phone = Base64Util.DecodeBase64(model.phone);
            string code = Base64Util.DecodeBase64(model.code);
            string newPwd = Base64Util.DecodeBase64(model.newPwd);
            string smsCodeToken = $"{CS.SYS_TYPE.MCH.ToLower()}_{CS.SMS_TYPE.RETRIEVE}_{phone}";
            string codeCacheKey = CS.GetCacheKeySmsCode(smsCodeToken);

#if !DEBUG
            string cacheCode = _redis.StringGet(codeCacheKey);
            if (string.IsNullOrWhiteSpace(cacheCode))
            {
                throw new BizException("��֤���ѹ��ڣ������µ��������֤�룡");
            }
            if (!cacheCode.Equals(code))
            {
                throw new BizException("��֤������");
            }
#endif
            var sysUser = _sysUserService.GetByTelphone(model.phone, CS.SYS_TYPE.MCH);
            if (sysUser == null)
            {
                throw new BizException("�û������ڣ�");
            }
            if (sysUser.State.Equals(CS.PUB_DISABLE))
            {
                throw new BizException("�û���ͣ�ã�");
            }
            var sysUserAuth = _sysUserAuthService.GetByIdentifier(CS.AUTH_TYPE.TELPHONE, model.phone, CS.SYS_TYPE.MCH);
            if (sysUserAuth == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            bool verified = BCryptUtil.VerifyHash(newPwd, sysUserAuth.Credential);
            if (verified)
            {
                throw new BizException("��������ԭ������ͬ��");
            }
            _sysUserAuthService.ResetAuthInfo(sysUser.SysUserId.Value, null, null, newPwd, CS.SYS_TYPE.MCH);
            // ɾ��������֤�뻺������
            _redis.KeyDelete(codeCacheKey);
            return ApiRes.Ok();
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("cipher/pwdRulesRegexp"), NoLog]
        public ApiRes PwdRulesRegexp()
        {
            var sysConfig = _sysConfigService.GetByKey("passwordRegexp", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            return ApiRes.Ok(JsonConvert.DeserializeObject<Dictionary<string, string>>(sysConfig.ConfigVal));
        }
    }
}