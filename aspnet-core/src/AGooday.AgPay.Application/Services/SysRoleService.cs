using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        public async Task AddAsync(SysRoleCreateDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            await _sysRoleRepository.AddAsync(entity);
            await _sysRoleRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(SysRoleModifyDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Update(entity);
            await _sysRoleRepository.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(string roleId)
        {
            if (await _sysUserRoleRelaRepository.IsAssignedToUserAsync(roleId))
            {
                throw new BizException("当前角色已分配到用户，不可删除！");
            }

            //删除当前表
            _sysRoleRepository.Remove(roleId);

            //删除关联表
            _sysRoleEntRelaRepository.RemoveByRoleId(roleId);

            await _sysRoleEntRelaRepository.SaveChangesAsync();
        }

        public async Task<SysRoleDto> GetByIdAsync(string recordId, string belongInfoId)
        {
            var entity = await _sysRoleRepository.GetAllAsNoTracking().Where(w => w.RoleId.Equals(recordId) && w.BelongInfoId.Equals(belongInfoId)).FirstOrDefaultAsync();
            var dto = _mapper.Map<SysRoleDto>(entity);
            return dto;
        }

        public async Task<PaginatedList<SysRoleDto>> GetPaginatedDataAsync(SysRoleQueryDto dto)
        {
            var sysRoles = _sysRoleRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.RoleName) || w.RoleName.Contains(dto.RoleName))
                && (string.IsNullOrWhiteSpace(dto.RoleId) || w.RoleId.Equals(dto.RoleId))
                && (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (string.IsNullOrWhiteSpace(dto.BelongInfoId) || w.BelongInfoId.Equals(dto.BelongInfoId))
                ).OrderByDescending(o => o.UpdatedAt);
            if (!string.IsNullOrEmpty(dto.SortField) && !string.IsNullOrEmpty(dto.SortOrder))
            {
                sysRoles = sysRoles.OrderBy(dto.SortField, dto.SortOrder.Equals(PageQuery.DESCEND, StringComparison.OrdinalIgnoreCase));
            }
            var records = await PaginatedList<SysRole>.CreateAsync<SysRoleDto>(sysRoles, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
