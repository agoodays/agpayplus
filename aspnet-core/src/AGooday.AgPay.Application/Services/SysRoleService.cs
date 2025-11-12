using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AGooday.AgPay.Infrastructure.Extensions.DataAccess;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统角色表 服务实现类
    /// </summary>
    public class SysRoleService : AgPayService<SysRoleDto, SysRole>, ISysRoleService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysRoleRepository _sysRoleRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;
        private readonly ISysRoleEntRelaRepository _sysRoleEntRelaRepository;

        public SysRoleService(IMapper mapper, IMediatorHandler bus,
            ISysRoleRepository sysRoleRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository,
            ISysRoleEntRelaRepository sysRoleEntRelaRepository)
            : base(mapper, bus, sysRoleRepository)
        {
            _sysRoleRepository = sysRoleRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
            _sysRoleEntRelaRepository = sysRoleEntRelaRepository;
        }

        public async Task<bool> AddAsync(SysRoleCreateDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            await _sysRoleRepository.AddAsync(entity);
            var (result, _) = await _sysRoleRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<bool> UpdateAsync(SysRoleModifyDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Update(entity);
            var (result, _) = await _sysRoleRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<bool> RemoveRoleAsync(string roleId)
        {
            if (await _sysUserRoleRelaRepository.IsAssignedToUserAsync(roleId))
            {
                throw new BizException("当前角色已分配到用户，不可删除！");
            }

            //删除当前表
            _sysRoleRepository.Remove(roleId);

            //删除关联表
            _sysRoleEntRelaRepository.RemoveByRoleId(roleId);

            var (result, _) = await _sysRoleEntRelaRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<SysRoleDto> GetByIdAsNoTrackingAsync(string recordId, string belongInfoId)
        {
            var query = _sysRoleRepository.GetAllAsNoTracking()
                .Where(w => w.RoleId.Equals(recordId) && w.BelongInfoId.Equals(belongInfoId));
            return await query.FirstOrDefaultProjectToAsync<SysRole, SysRoleDto>(_mapper);
        }

        public Task<PaginatedResult<SysRoleDto>> GetPaginatedDataAsync(SysRoleQueryDto dto)
        {
            var query = _sysRoleRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.RoleName, w => w.RoleName.Contains(dto.RoleName))
                .WhereIfNotEmpty(dto.RoleId, w => w.RoleId.Equals(dto.RoleId))
                .WhereIfNotEmpty(dto.SysType, w => w.SysType.Equals(dto.SysType))
                .WhereIfNotEmpty(dto.BelongInfoId, w => w.BelongInfoId.Equals(dto.BelongInfoId))
                .OrderByDescending(o => o.UpdatedAt);
            if (!string.IsNullOrEmpty(dto.SortField) && !string.IsNullOrEmpty(dto.SortOrder))
            {
                query = query.OrderBy(dto.SortField, dto.SortOrder.Equals(PageQuery.DESCEND, StringComparison.OrdinalIgnoreCase));
            }
            return query.ToPaginatedResultAsync<SysRole, SysRoleDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
