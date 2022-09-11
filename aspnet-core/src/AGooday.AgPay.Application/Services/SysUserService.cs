using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
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

        public SysUserService(ISysUserRepository sysUserRepository, IMapper mapper)
        {
            _sysUserRepository = sysUserRepository;
            _mapper = mapper;
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

        public IEnumerable<SysUserVM> GetAll()
        {
            //第一种写法 Map
            var sysUsers = _sysUserRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserVM>>(sysUsers);

            //第二种写法 ProjectTo
            //return (_UsersRepository.GetAll()).ProjectTo<UsersViewModel>(_mapper.ConfigurationProvider);
        }

        public Task<IEnumerable<SysUserVM>> ListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
