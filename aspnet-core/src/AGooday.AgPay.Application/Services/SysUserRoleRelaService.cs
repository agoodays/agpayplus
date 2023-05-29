using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 操作员<->角色 关联表 服务实现类
    /// </summary>
    public class SysUserRoleRelaService : ISysUserRoleRelaService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserRoleRelaService(IMapper mapper, IMediatorHandler bus, ISysUserRoleRelaRepository sysUserRoleRelaRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysUserRoleRelaDto dto)
        {
            var m = _mapper.Map<SysUserRoleRela>(dto);
            _sysUserRoleRelaRepository.Add(m);
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysUserRoleRelaRepository.Remove(recordId);
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public void Update(SysUserRoleRelaDto dto)
        {
            var m = _mapper.Map<SysUserRoleRela>(dto);
            _sysUserRoleRelaRepository.Update(m);
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public void SaveUserRole(long userId, List<string> roleIds) {

            foreach (var roleId in roleIds)
            {
                var m = new SysUserRoleRela() { UserId = userId, RoleId = roleId };
                _sysUserRoleRelaRepository.Add(m);
            }
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public SysUserRoleRelaDto GetById(string recordId)
        {
            var entity = _sysUserRoleRelaRepository.GetById(recordId);
            var dto = _mapper.Map<SysUserRoleRelaDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserRoleRelaDto> GetAll()
        {
            var sysUserRoleRelas = _sysUserRoleRelaRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserRoleRelaDto>>(sysUserRoleRelas);
        }

        public PaginatedList<SysUserRoleRelaDto> GetPaginatedData(SysUserRoleRelaQueryDto dto)
        {
            var sysUserRoleRelas = _sysUserRoleRelaRepository.GetAll()
                .Where(w => dto.UserId.Equals(0) || w.UserId.Equals(dto.UserId));
            var records = PaginatedList<SysUserRoleRela>.Create<SysUserRoleRelaDto>(sysUserRoleRelas.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        /// <summary>
        /// 根据用户查询全部角色集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectRoleIdsByUserId(long userId)
        {
            return _sysUserRoleRelaRepository.GetAll()
                .Where(w => w.UserId == userId)
                .Select(s => s.RoleId);
        }

        /// <summary>
        /// 根据角色查询全部用户集合
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<long> SelectRoleIdsByRoleId(string roleId)
        {
            return _sysUserRoleRelaRepository.GetAll()
                .Where(w => w.RoleId.Equals(roleId))
                .Select(s => s.UserId);
        }
    }
}
