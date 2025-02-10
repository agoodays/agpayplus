using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Extensions;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
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
        // ������֪ͨ�������ע��Controller
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
        /// ��ǰ�û���Ϣ
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizeException"></exception>
        [HttpGet, Route("user"), NoLog]
        public async Task<ApiRes> CurrentUserInfoAsync()
        {
            try
            {
                //��ǰ�û���Ϣ
                var currentUser = await GetCurrentUserAsync();
                var user = currentUser.SysUser;

                //1. ��ǰ�û�����Ȩ��ID����
                var entIds = currentUser.Authorities.ToList();

                //2. ��ѯ���û����в˵����� (���������ʾ�˵� �� �������Ͳ˵� )
                var sysEnts = _authService.GetEntsBySysType(user.SysType, entIds, new List<string> { CS.ENT_TYPE.MENU_LEFT, CS.ENT_TYPE.MENU_OTHER });

                //�ݹ�ת��Ϊ��״�ṹ
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
                //1. ����Ȩ��ID����
                user.AddExt("entIdList", entIds);
                user.AddExt("allMenuRouteTree", allMenuRouteTree);
                return ApiRes.Ok(user);
            }
            catch (Exception)
            {
                throw new UnauthorizeException();
                //throw new BizException("��¼ʧЧ");
                //return ApiRes.CustomFail("��¼ʧЧ");
            }
        }

        /// <summary>
        /// ��ǰ�û�ɨ��
        /// </summary>
        /// <param name="qrcodeNo"></param>
        /// <returns></returns>
        [HttpGet, Route("user/scan"), NoLog]
        public async Task<ApiRes> ScanAsync(string qrcodeNo)
        {
            string loginQRCacheKey = CS.GetCacheKeyLoginQR(qrcodeNo);
            if (!await _cacheService.ExistsAsync(loginQRCacheKey))
            {
                throw new BizException("��ά����Ч����ˢ�¶�ά�������ɨ��");
            }
            var qrcodeInfo = await _cacheService.GetAsync<dynamic>(loginQRCacheKey);
            if (qrcodeInfo == null || qrcodeInfo.qrcodeStatus != CS.QR_CODE_STATUS.WAITING)
            {
                throw new BizException("��ά��״̬��Ч����ˢ�¶�ά�������ɨ��");
            }
            await _cacheService.UpdateWithExistingExpiryAsync(loginQRCacheKey, new { qrcodeStatus = CS.QR_CODE_STATUS.SCANNED });
            return ApiRes.Ok();
        }

        /// <summary>
        /// ��ǰ�û�ȷ�ϵ�¼
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
                throw new BizException("��ά����Ч����ˢ�¶�ά�������ɨ��");
            }
            var qrcodeInfo = await _cacheService.GetAsync<dynamic>(loginQRCacheKey);
            if (qrcodeInfo == null || qrcodeInfo.qrcodeStatus != CS.QR_CODE_STATUS.SCANNED)
            {
                throw new BizException("��ά��״̬��Ч����ˢ�¶�ά�������ɨ��");
            }
            if (isConfirm)
            {
                // ��ȡ��Ȩͷ��ֵ
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
        /// �޸ĸ�����Ϣ
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("user"), MethodLog("�޸ĸ�����Ϣ")]
        public async Task<ApiRes> ModifyCurrentUserInfoAsync(ModifyCurrentUserInfoDto dto)
        {
            var currentUser = await GetCurrentUserAsync();
            dto.SysUserId = currentUser.SysUser.SysUserId;
            await _sysUserService.ModifyCurrentUserInfoAsync(dto);
            var userinfo = await _authService.GetUserAuthInfoByIdAsync(currentUser.SysUser.SysUserId);
            currentUser.SysUser = userinfo;
            //����redis��������
            await _cacheService.SetAsync(currentUser.CacheKey, currentUser, new TimeSpan(0, 0, CS.TOKEN_TIME));
            return ApiRes.Ok();
        }

        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPut, Route("modifyPwd"), MethodLog("�޸�����")]
        public async Task<ApiRes> ModifyPwdAsync(ModifyPwd model)
        {
            var currentUser = await GetCurrentUserAsync();
            string currentUserPwd = Base64Util.DecodeBase64(model.OriginalPwd); //��ǰ�û���¼����
            var user = await _authService.GetUserAuthInfoByIdAsync(currentUser.SysUser.SysUserId);
            bool verified = BCryptUtil.VerifyHash(currentUserPwd, user.Credential);
            //��֤��ǰ�����Ƿ���ȷ
            if (!verified)
            {
                throw new BizException("ԭ������֤ʧ�ܣ�");
            }
            string opUserPwd = Base64Util.DecodeBase64(model.ConfirmPwd);
            // ��֤ԭ�������������Ƿ���ͬ
            if (opUserPwd.Equals(currentUserPwd))
            {
                throw new BizException("��������ԭ���벻����ͬ��");
            }
            await _sysUserAuthService.ResetAuthInfoAsync(user.SysUserId, null, null, opUserPwd, user.SysType);
            return await LogoutAsync();
        }

        /// <summary>
        /// �˳���¼
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("logout"), MethodLog("�˳���¼")]
        public async Task<ApiRes> LogoutAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            await _cacheService.RemoveAsync(currentUser.CacheKey);
            return ApiRes.Ok();
        }
    }
}