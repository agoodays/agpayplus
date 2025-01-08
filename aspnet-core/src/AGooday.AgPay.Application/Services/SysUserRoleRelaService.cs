using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 操作员<->角色 关联表 服务实现类
    /// </summary>
    public class SysUserRoleRelaService : AgPayService<SysUserRoleRelaDto, SysUserRoleRela>, ISysUserRoleRelaService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;

        public SysUserRoleRelaService(IMapper mapper, IMediatorHandler bus,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository)
            : base(mapper, bus, sysUserRoleRelaRepository)
        {
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }

        public async Task<int> SaveUserRoleAsync(long userId, List<string> roleIds)
        {
            var entities = roleIds.Select(roleId => new SysUserRoleRela() { UserId = userId, RoleId = roleId }).AsQueryable();
            await _sysUserRoleRelaRepository.AddRangeAsync(entities);
            return await _sysUserRoleRelaRepository.SaveChangesAsync();
        }

        public Task<PaginatedList<SysUserRoleRelaDto>> GetPaginatedDataAsync(SysUserRoleRelaQueryDto dto)
        {
            var query = _sysUserRoleRelaRepository.GetAllAsNoTracking()
                .Where(w => dto.UserId.Equals(null) || w.UserId.Equals(dto.UserId));
            var records = PaginatedList<SysUserRoleRela>.CreateAsync<SysUserRoleRelaDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        /// <summary>
        /// 根据用户查询全部角色集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectRoleIdsByUserId(long userId)
        {
            return _sysUserRoleRelaRepository.GetAllAsNoTracking()
                .Where(w => w.UserId == userId)
                .Select(s => s.RoleId);
        }

        /// <summary>
        /// 根据角色查询全部用户集合
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<long> SelectUserIdsByRoleId(string roleId)
        {
            return _sysUserRoleRelaRepository.GetAllAsNoTracking()
                .Where(w => w.RoleId.Equals(roleId))
                .Select(s => s.UserId);
        }
    }
}
