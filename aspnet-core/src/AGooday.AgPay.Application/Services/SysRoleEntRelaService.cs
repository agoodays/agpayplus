using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
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
    public class SysRoleEntRelaService : ISysRoleEntRelaService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysRoleEntRelaRepository _sysRoleEntRelaRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysRoleEntRelaService(ISysRoleEntRelaRepository sysRoleEntRelaRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysRoleEntRelaRepository = sysRoleEntRelaRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysRoleEntRelaVM vm)
        {
            var m = _mapper.Map<SysRoleEntRela>(vm);
            _sysRoleEntRelaRepository.Add(m);
        }

        public void Remove(string recordId)
        {
            _sysRoleEntRelaRepository.Remove(recordId);
        }

        public void Update(SysRoleEntRelaVM vm)
        {
            var m = _mapper.Map<SysRoleEntRela>(vm);
            _sysRoleEntRelaRepository.Update(m);
        }

        public SysRoleEntRelaVM GetById(string recordId)
        {
            var entity = _sysRoleEntRelaRepository.GetById(recordId);
            var vm = _mapper.Map<SysRoleEntRelaVM>(entity);
            return vm;
        }

        public IEnumerable<SysRoleEntRelaVM> GetAll()
        {
            var sysRoleEntRelas = _sysRoleEntRelaRepository.GetAll();
            return _mapper.Map<IEnumerable<SysRoleEntRelaVM>>(sysRoleEntRelas);
        }
    }
}
