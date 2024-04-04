using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统角色权限关联表 服务实现类
    /// </summary>
    public class SysRoleEntRelaService : AgPayService<SysRoleEntRelaDto, SysRoleEntRela>, ISysRoleEntRelaService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysRoleEntRelaRepository _sysRoleEntRelaRepository;
        private readonly ISysEntitlementRepository _sysEntitlementRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;

        public SysRoleEntRelaService(IMapper mapper, IMediatorHandler bus,
            ISysRoleEntRelaRepository sysRoleEntRelaRepository,
            ISysEntitlementRepository sysEntitlementRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository)
            : base(mapper, bus, sysRoleEntRelaRepository)
        {
            _sysRoleEntRelaRepository = sysRoleEntRelaRepository;
            _sysEntitlementRepository = sysEntitlementRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }


        public PaginatedList<SysRoleEntRelaDto> GetPaginatedData(SysRoleEntRelaQueryDto dto)
        {
            var sysRoleEntRelas = _sysRoleEntRelaRepository.GetAllAsNoTracking()
                .Where(w => string.IsNullOrWhiteSpace(dto.RoleId) || w.RoleId.Equals(dto.RoleId));
            var records = PaginatedList<SysRoleEntRela>.Create<SysRoleEntRelaDto>(sysRoleEntRelas, _mapper, dto.PageNumber, dto.PageSize);
            return records;
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
        /// 根据人查询出所有权限ID集合
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectEntIdsByUserId(long userId, byte userType, string sysType)
        {
            return SelectEntsByUserId(userId, userType, sysType).Select(s => s.EntId);
        }

        /// <summary>
        /// 根据人查询出所有权限集合
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public IEnumerable<SysEntitlementDto> SelectEntsByUserId(long userId, byte userType, string sysType)
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

        /// <summary>
        /// 重置 角色 - 权限 关联关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="entIdList"></param>
        public void ResetRela(string roleId, List<string> entIdList)
        {
            //1. 删除
            _sysRoleEntRelaRepository.RemoveByRoleId(roleId);

            //2. 插入
            entIdList.ForEach((entId) =>
            {
                var m = new SysRoleEntRela()
                {
                    RoleId = roleId,
                    EntId = entId,
                };
                _sysRoleEntRelaRepository.Add(m);
            });

            _sysRoleEntRelaRepository.SaveChanges();
        }
    }
}
