using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Services
{
    public class SysUserService : ISysUserService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserRepository _sysUserRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserService(ISysUserRepository sysUserRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysUserRepository = sysUserRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysUserVM vm)
        {
            var m = _mapper.Map<SysUser>(vm);
            _sysUserRepository.Add(m);
        }

        public void Create(SysUserVM vm)
        {
            var command = _mapper.Map<CreateSysUserCommand>(vm);
            Bus.SendCommand(command);
        }

        public void Remove(long recordId)
        {
            _sysUserRepository.Remove(recordId);
        }

        public void Update(SysUserVM vm)
        {
            var m = _mapper.Map<SysUser>(vm);
            _sysUserRepository.Update(m);
        }

        public SysUserVM GetById(long recordId)
        {
            var entity = _sysUserRepository.GetById(recordId);
            var vm = _mapper.Map<SysUserVM>(entity);
            return vm;
        }

        public IEnumerable<SysUserVM> GetAll()
        {
            //第一种写法 Map
            var sysUsers = _sysUserRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserVM>>(sysUsers);

            //第二种写法 ProjectTo
            //return (_UsersRepository.GetAll()).ProjectTo<SysUserVM>(_mapper.ConfigurationProvider);
        }

        public Task<IEnumerable<SysUserVM>> ListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
