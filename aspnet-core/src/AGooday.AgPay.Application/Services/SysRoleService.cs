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

        public async Task<bool> AddAsync(SysRoleCreateDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            await _sysRoleRepository.AddAsync(entity);
            return await _sysRoleRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(SysRoleModifyDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Update(entity);
            return await _sysRoleRepository.SaveChangesAsync() > 0;
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

            return await _sysRoleEntRelaRepository.SaveChangesAsync() > 0;
        }

        public async Task<SysRoleDto> GetByIdAsync(string recordId, string belongInfoId)
        {
            var entity = await _sysRoleRepository.GetAllAsNoTracking().Where(w => w.RoleId.Equals(recordId) && w.BelongInfoId.Equals(belongInfoId)).FirstOrDefaultAsync();
            var dto = _mapper.Map<SysRoleDto>(entity);
            return dto;
        }

        public Task<PaginatedList<SysRoleDto>> GetPaginatedDataAsync(SysRoleQueryDto dto)
        {
            var query = _sysRoleRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.RoleName) || w.RoleName.Contains(dto.RoleName))
                && (string.IsNullOrWhiteSpace(dto.RoleId) || w.RoleId.Equals(dto.RoleId))
                && (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (string.IsNullOrWhiteSpace(dto.BelongInfoId) || w.BelongInfoId.Equals(dto.BelongInfoId)))
                .OrderByDescending(o => o.UpdatedAt);
            if (!string.IsNullOrEmpty(dto.SortField) && !string.IsNullOrEmpty(dto.SortOrder))
            {
                query = query.OrderBy(dto.SortField, dto.SortOrder.Equals(PageQuery.DESCEND, StringComparison.OrdinalIgnoreCase));
            }
            var records = PaginatedList<SysRole>.CreateAsync<SysRoleDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
