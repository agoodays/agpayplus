using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("api/current")]
    [ApiController]
    public class CurrentUserController : CommonController
    {
        private readonly IDatabase _redis;
        private readonly ISysUserService _sysUserService;
        private readonly ISysUserAuthService _sysUserAuthService;
        private readonly IMemoryCache _cache;
        private readonly IAuthService _authService;
        // ������֪ͨ�������ע��Controller
        private readonly DomainNotificationHandler _notifications;

        public CurrentUserController(ILogger<CurrentUserController> logger,
            IMemoryCache cache,
            ISysUserService sysUserService,
            ISysUserAuthService sysUserAuthService,
            INotificationHandler<DomainNotification> notifications,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysUserService = sysUserService;
            _sysUserAuthService = sysUserAuthService;
            _cache = cache;
            _redis = client.GetDatabase();
            _authService = authService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// ��ǰ�û���Ϣ
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizeException"></exception>
        [HttpGet, Route("user"), NoLog]
        public ApiRes CurrentUserInfo()
        {
            try
            {
                //��ǰ�û���Ϣ
                var currentUser = GetCurrentUser();
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
        /// �޸ĸ�����Ϣ
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("user"), MethodLog("�޸ĸ�����Ϣ")]
        public async Task<ApiRes> ModifyCurrentUserInfoAsync(ModifyCurrentUserInfoDto dto)
        {
            var currentUser = GetCurrentUser();
            dto.SysUserId = currentUser.SysUser.SysUserId;
            await _sysUserService.ModifyCurrentUserInfoAsync(dto);
            var userinfo = await _authService.GetUserAuthInfoByIdAsync(currentUser.SysUser.SysUserId);
            currentUser.SysUser = userinfo;
            //����redis��������
            var currentUserJson = JsonConvert.SerializeObject(currentUser);
            await _redis.StringSetAsync(currentUser.CacheKey, currentUserJson, new TimeSpan(0, 0, CS.TOKEN_TIME));
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
            var currentUser = GetCurrentUser();
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
            var currentUser = GetCurrentUser();
            await _redis.KeyDeleteAsync(currentUser.CacheKey);
            return ApiRes.Ok();
        }
    }
}