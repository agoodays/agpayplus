using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public SysUserService(IMapper mapper, IMediatorHandler bus, IConfiguration configuration,
            ISysUserRepository sysUserRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysUserRepository = sysUserRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysUserDto dto)
        {
            var m = _mapper.Map<SysUser>(dto);
            _sysUserRepository.Add(m);
            _sysUserRepository.SaveChanges();
        }

        public void Create(SysUserDto dto)
        {
            var command = _mapper.Map<CreateSysUserCommand>(dto);
            Bus.SendCommand(command);
        }

        public void Remove(long recordId)
        {
            _sysUserRepository.Remove(recordId);
            _sysUserRepository.SaveChanges();
        }

        public void Remove(long sysUserId, long currentUserId, string sysType)
        {
            var command = new RemoveSysUserCommand()
            {
                SysUserId = sysUserId,
                CurrentSysUserId = currentUserId,
                SysType = sysType
            };
            Bus.SendCommand(command);
        }

        public void Update(SysUserDto dto)
        {
            var m = _mapper.Map<SysUser>(dto);
            _sysUserRepository.Update(m);
            _sysUserRepository.SaveChanges();
        }

        public void Modify(ModifySysUserDto dto)
        {
            var command = _mapper.Map<ModifySysUserCommand>(dto);
            Bus.SendCommand(command);
        }

        public SysUserDto GetById(long recordId)
        {
            var entity = _sysUserRepository.GetById(recordId);
            var dto = _mapper.Map<SysUserDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserDto> GetAll()
        {
            //第一种写法 Map
            var sysUsers = _sysUserRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserDto>>(sysUsers);

            //第二种写法 ProjectTo
            //return (_UsersRepository.GetAll()).ProjectTo<SysUserVM>(_mapper.ConfigurationProvider);
        }

        public IEnumerable<SysUserDto> GetAll(List<long> recordIds)
        {
            var sysUsers = _sysUserRepository.GetAll().Where(w => recordIds.Contains(w.SysUserId));
            return _mapper.Map<IEnumerable<SysUserDto>>(sysUsers);
        }

        public PaginatedList<SysUserDto> GetPaginatedData(SysUserDto dto, int pageIndex = 1, int pageSize = 20)
        {
            var sysUsers = _sysUserRepository.GetAll()
                .Where(w => w.SysType == dto.SysType
                && (string.IsNullOrWhiteSpace(dto.Realname) || w.Realname.Contains(dto.Realname))
                && (dto.SysUserId.Equals(0) || w.SysUserId.Equals(dto.SysUserId))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysUser>.Create<SysUserDto>(sysUsers.AsNoTracking(), _mapper, pageIndex, pageSize);
            return records;
        }

        public Task<IEnumerable<SysUserDto>> ListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
