using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
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

        /// <summary>
        /// 分配用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<int> SaveUserRoleAsync(long userId, List<string> roleIds)
        {
            // 删除用户之前的 角色信息
            _sysUserRoleRelaRepository.RemoveByUserId(userId);
            var entities = roleIds.Select(roleId => new SysUserRoleRela() { UserId = userId, RoleId = roleId }).AsQueryable();
            await _sysUserRoleRelaRepository.AddRangeAsync(entities);
            return await _sysUserRoleRelaRepository.SaveChangesAsync();
        }

        public Task<PaginatedResult<SysUserRoleRelaDto>> GetPaginatedDataAsync(SysUserRoleRelaQueryDto dto)
        {
            var query = _sysUserRoleRelaRepository.GetAllAsNoTracking()
                .Where(w => dto.UserId.Equals(null) || w.UserId.Equals(dto.UserId));
            return query.ToPaginatedResultAsync<SysUserRoleRela, SysUserRoleRelaDto>(_mapper, dto.PageNumber, dto.PageSize);
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
