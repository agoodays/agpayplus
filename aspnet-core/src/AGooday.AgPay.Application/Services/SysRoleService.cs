using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统角色表 服务实现类
    /// </summary>
    public class SysRoleService : ISysRoleService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysRoleRepository _sysRoleRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;
        private readonly ISysRoleEntRelaRepository _sysRoleEntRelaRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysRoleService(IMapper mapper, IMediatorHandler bus,
            ISysRoleRepository sysRoleRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository,
            ISysRoleEntRelaRepository sysRoleEntRelaRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysRoleRepository = sysRoleRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
            _sysRoleEntRelaRepository = sysRoleEntRelaRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysRoleCreateDto dto)
        {
            var m = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Add(m);
            _sysRoleRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysRoleRepository.Remove(recordId);
            _sysRoleRepository.SaveChanges();
        }

        public void Update(SysRoleModifyDto dto)
        {
            var m = _mapper.Map<SysRole>(dto);
            _sysRoleRepository.Update(m);
            _sysRoleRepository.SaveChanges();
        }

        public SysRoleDto GetById(string recordId)
        {
            var entity = _sysRoleRepository.GetById(recordId);
            var dto = _mapper.Map<SysRoleDto>(entity);
            return dto;
        }

        public SysRoleDto GetById(string recordId, string belongInfoId)
        {
            var entity = _sysRoleRepository.GetAll().Where(w => w.RoleId.Equals(recordId) && w.BelongInfoId.Equals(belongInfoId)).FirstOrDefault();
            var dto = _mapper.Map<SysRoleDto>(entity);
            return dto;
        }

        public IEnumerable<SysRoleDto> GetAll()
        {
            var sysRoles = _sysRoleRepository.GetAll();
            return _mapper.Map<IEnumerable<SysRoleDto>>(sysRoles);
        }

        public PaginatedList<SysRoleDto> GetPaginatedData(SysRoleQueryDto dto)
        {
            var sysRoles = _sysRoleRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.RoleName) || w.RoleName.Contains(dto.RoleName))
                && (string.IsNullOrWhiteSpace(dto.RoleId) || w.RoleId.Equals(dto.RoleId))
                && (string.IsNullOrWhiteSpace(dto.SysType) || w.SysType.Equals(dto.SysType))
                && (string.IsNullOrWhiteSpace(dto.BelongInfoId) || w.BelongInfoId.Equals(dto.BelongInfoId))
                ).OrderByDescending(o => o.UpdatedAt);
            var records = PaginatedList<SysRole>.Create<SysRoleDto>(sysRoles.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
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
    }
}
