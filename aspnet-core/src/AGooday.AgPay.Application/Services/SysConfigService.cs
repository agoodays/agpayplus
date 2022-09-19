using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
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
    public class SysConfigService : ISysConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysConfigRepository _sysConfigRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysConfigService(ISysConfigRepository sysConfigRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysConfigRepository = sysConfigRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysConfigDto dto)
        {
            var m = _mapper.Map<SysConfig>(dto);
            _sysConfigRepository.Add(m);
            _sysConfigRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysConfigRepository.Remove(recordId);
            _sysConfigRepository.SaveChanges();
        }

        public void Update(SysConfigDto dto)
        {
            var m = _mapper.Map<SysConfig>(dto);
            _sysConfigRepository.Update(m);
            _sysConfigRepository.SaveChanges();
        }

        public SysConfigDto GetById(string recordId)
        {
            var entity = _sysConfigRepository.GetById(recordId);
            var dto = _mapper.Map<SysConfigDto>(entity);
            return dto;
        }

        public IEnumerable<SysConfigDto> GetAll()
        {
            var sysConfigs = _sysConfigRepository.GetAll();
            return _mapper.Map<IEnumerable<SysConfigDto>>(sysConfigs);
        }
    }
}
