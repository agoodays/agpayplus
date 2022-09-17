using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Add(SysRoleVM vm)
        {
            var m = _mapper.Map<SysRole>(vm);
            _sysRoleRepository.Add(m);
            _sysRoleRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysRoleRepository.Remove(recordId);
            _sysRoleRepository.SaveChanges();
        }

        public void Update(SysRoleVM vm)
        {
            var m = _mapper.Map<SysRole>(vm);
            _sysRoleRepository.Update(m);
            _sysRoleRepository.SaveChanges();
        }

        public SysRoleVM GetById(string recordId)
        {
            var entity = _sysRoleRepository.GetById(recordId);
            var vm = _mapper.Map<SysRoleVM>(entity);
            return vm;
        }

        public IEnumerable<SysRoleVM> GetAll()
        {
            var sysRoles = _sysRoleRepository.GetAll();
            return _mapper.Map<IEnumerable<SysRoleVM>>(sysRoles);
        }

        public void RemoveRole(string roleId)
        {
            if (_sysUserRoleRelaRepository.IsAssignedToUser(roleId))
            {
                throw new BizException("当前角色已分配到用户， 不可删除！");
            }

            //删除当前表
            _sysRoleRepository.Remove(roleId);

            //删除关联表
            _sysRoleEntRelaRepository.RemoveByRoleId(roleId);

            _sysRoleEntRelaRepository.SaveChanges();
        }
    }
}
