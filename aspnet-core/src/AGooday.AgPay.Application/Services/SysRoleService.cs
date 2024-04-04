using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
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

        public void Add(SysRoleCreateDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Add(entity);
            _sysRoleRepository.SaveChanges();
        }

        public void Update(SysRoleModifyDto dto)
        {
            var entity = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Update(entity);
            _sysRoleRepository.SaveChanges();
        }

        public void RemoveRole(string roleId)
        {
            if (_sysUserRoleRelaRepository.IsAssignedToUser(roleId))
            {
                throw new BizException("当前角色已分配到用户，不可删除！");
            }

            //删除当前表
            _sysRoleRepository.Remove(roleId);

            //删除关联表
            _sysRoleEntRelaRepository.RemoveByRoleId(roleId);

            _sysRoleEntRelaRepository.SaveChanges();
        }

        public SysRoleDto GetById(string recordId, string belongInfoId)
        {
            var entity = _sysRoleRepository.GetAll().Where(w => w.RoleId.Equals(recordId) && w.BelongInfoId.Equals(belongInfoId)).FirstOrDefault();
            var dto = _mapper.Map<SysRoleDto>(entity);
            return dto;
        }

        public PaginatedList<SysRoleDto> GetPaginatedData(SysRoleQueryDto dto)
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
            var records = PaginatedList<SysRole>.Create<SysRoleDto>(sysRoles, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
