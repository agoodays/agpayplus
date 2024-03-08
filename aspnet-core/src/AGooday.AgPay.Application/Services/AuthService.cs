using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Services
{
    public class AuthService : IAuthService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly ISysEntitlementRepository _sysEntitlementRepository;
        private readonly ISysRoleEntRelaRepository _sysRoleEntRelaRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;

        public AuthService(IMapper mapper,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository,
            ISysEntitlementRepository sysEntitlementRepository,
            ISysRoleEntRelaRepository sysRoleEntRelaRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository,
            IMchInfoRepository mchInfoRepository,
            IAgentInfoRepository agentInfoRepository)
        {
            _sysUserRepository = sysUserRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
            _sysEntitlementRepository = sysEntitlementRepository;
            _sysRoleEntRelaRepository = sysRoleEntRelaRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
            _mchInfoRepository = mchInfoRepository;
            _agentInfoRepository = agentInfoRepository;
            _mapper = mapper;
        }

        public SysUserDto GetUserById(long userId)
        {
            var entity = _sysUserRepository.GetById(userId);
            var dto = _mapper.Map<SysUserDto>(entity);
            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordIds"></param>
        /// <returns></returns>
        public IEnumerable<SysUserDto> GetUserByIds(List<long> userIds)
        {
            var sysUsers = _sysUserRepository.GetAll().Where(w => userIds.Contains(w.SysUserId));
            return _mapper.Map<IEnumerable<SysUserDto>>(sysUsers);
        }

        /// <summary>
        /// 根据用户查询全部角色集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<SysUserRoleRelaDto> GetUserRolesByUserId(long userId)
        {
            var userRoles = _sysUserRoleRelaRepository.GetAll()
                .Where(w => w.UserId == userId);
            return _mapper.Map<IEnumerable<SysUserRoleRelaDto>>(userRoles);
        }

        public IEnumerable<SysEntitlementDto> GetEntsBySysType(string sysType, string entId)
        {
            var sysEnts = _sysEntitlementRepository.GetAll()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (string.IsNullOrWhiteSpace(entId) || w.EntId.Equals(entId))
                );
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEnts);
        }

        public IEnumerable<SysEntitlementDto> GetEntsBySysType(string sysType, List<string> entIds, List<string> entTypes)
        {
            var sysEnts = _sysEntitlementRepository.GetAll()
                .Where(w => w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE)
                && (!(entIds != null && entIds.Count > 0) || entIds.Contains(w.EntId))
                && (!(entTypes != null && entTypes.Count > 0) || entTypes.Contains(w.EntType))
                );
            return _mapper.Map<IEnumerable<SysEntitlementDto>>(sysEnts);
        }

        /// <summary>
        /// 根据人查询出所有权限集合
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public IEnumerable<SysEntitlementDto> GetEntsByUserId(long userId, byte userType, string sysType)
        {
            if (userType == CS.USER_TYPE.ADMIN)
            {
                var result = _sysEntitlementRepository.GetAll()
                    .Where(w => w.SysType.Equals(sysType) && w.State == CS.PUB_USABLE);
                return _mapper.Map<IEnumerable<SysEntitlementDto>>(result);
            }
            else
            {
                var result = _sysUserRoleRelaRepository.GetAll()
                    .Join(_sysUserRoleRelaRepository.GetAll<SysRoleEntRela>(),
                    ur => ur.RoleId, re => re.RoleId,
                    (ur, re) => new { ur.UserId, re.EntId })
                    .Join(_sysUserRoleRelaRepository.GetAll<SysEntitlement>(),
                        ue => ue.EntId, ent => ent.EntId,
                        (ue, ent) => new { ue.UserId, ent })
                    .Where(w => w.UserId.Equals(userId) && w.ent.SysType.Equals(sysType) && w.ent.State.Equals(CS.PUB_USABLE))
                    .Select(s => s.ent);
                return _mapper.Map<IEnumerable<SysEntitlementDto>>(result);
            }
        }

        public SysUserAuthInfoDto GetUserAuthInfoById(long userId)
        {
            var auth = _sysUserAuthRepository.GetAll()
                .Join(_sysUserAuthRepository.GetAll<SysUser>(),
                ua => ua.UserId, ur => ur.SysUserId,
                (ua, ur) => new { ua, ur })
                .Where(w => w.ua.UserId == userId)
                .FirstOrDefault();

            return auth == null ? null : GetSysUserAuthInfo(auth.ua, auth.ur);
        }

        private SysUserAuthInfoDto GetSysUserAuthInfo(SysUserAuth ua, SysUser ur)
        {
            var auth = new SysUserAuthInfoDto
            {
                SysUserId = ur.SysUserId,
                LoginUsername = ur.LoginUsername,
                Realname = ur.Realname,
                Telphone = ur.Telphone,
                Sex = ur.Sex,
                AvatarUrl = ur.AvatarUrl,
                UserNo = ur.UserNo,
                SafeWord = ur.SafeWord,
                IsAdmin = ur.UserType.Equals(CS.USER_TYPE.ADMIN) ? CS.YES : CS.NO,
                UserType = ur.UserType,
                EntRules = string.IsNullOrWhiteSpace(ur.EntRules) ? null : JsonConvert.DeserializeObject<List<string>>(ur.EntRules),
                BindStoreIds = string.IsNullOrWhiteSpace(ur.BindStoreIds) ? null : JsonConvert.DeserializeObject<List<long>>(ur.BindStoreIds),
                InviteCode = ur.InviteCode,
                TeamId = ur.TeamId,
                IsTeamLeader = ur.IsTeamLeader,
                State = ur.State,
                SysType = ur.SysType,
                BelongInfoId = ur.BelongInfoId,
                CreatedAt = ur.CreatedAt,
                UpdatedAt = ur.UpdatedAt,
                IdentityType = ua.IdentityType,
                Identifier = ua.Identifier,
                Credential = ua.Credential
            };

            if (auth != null && ur.SysType.Equals(CS.SYS_TYPE.MCH))
            {
                var mch = _mchInfoRepository.GetById(auth.BelongInfoId);
                auth.ShortName = mch.MchShortName;
                auth.MchType = mch.Type;
                auth.MchLevel = mch.MchLevel;
            }

            if (auth != null && ur.SysType.Equals(CS.SYS_TYPE.AGENT))
            {
                var agent = _agentInfoRepository.GetById(auth.BelongInfoId);
                auth.ShortName = agent.AgentShortName;
            }

            return auth;
        }

        /// <summary>
        /// 查询当前用户是否存在左侧菜单 (仅普通操作员)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public bool UserHasLeftMenu(long userId, string sysType)
        {
            var result = _sysRoleEntRelaRepository.GetAllAsNoTracking<SysUserRoleRela>()
                .Join(_sysRoleEntRelaRepository.GetAllAsNoTracking(),
                ur => ur.RoleId, re => re.RoleId,
                (ur, re) => new { ur.UserId, re.EntId })
                .Join(_sysRoleEntRelaRepository.GetAll<SysEntitlement>(),
                    ue => ue.EntId, ent => ent.EntId,
                    (ue, ent) => new { ue.UserId, ent.EntId, ent.EntType, ent.SysType, ent.State })
                .Any(w => w.UserId.Equals(userId)
                && w.SysType.Equals(sysType) && w.State.Equals(CS.PUB_USABLE) && w.EntType.Equals(CS.ENT_TYPE.MENU_LEFT));

            return result;
        }

        /// <summary>
        /// 根据登录信息查询用户认证信息
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="identityType"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public SysUserAuthInfoDto LoginAuth(string identifier, byte identityType, string sysType)
        {
            var auth = _sysUserAuthRepository.GetAll()
                .Join(_sysUserRepository.GetAll(),
                ua => ua.UserId, ur => ur.SysUserId,
                (ua, ur) => new { ua, ur })
                .Where(w => w.ua.IdentityType == identityType && w.ua.Identifier.Equals(identifier) && w.ua.SysType.Equals(sysType) && w.ur.State == CS.PUB_USABLE)
                .FirstOrDefault();

            return auth == null ? null : GetSysUserAuthInfo(auth.ua, auth.ur);
        }
    }
}
