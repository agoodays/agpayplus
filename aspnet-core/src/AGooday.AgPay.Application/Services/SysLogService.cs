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
    public class SysLogService : ISysLogService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysLogRepository _sysLogRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysLogService(ISysLogRepository sysLogRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysLogRepository = sysLogRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysLogDto dto)
        {
            var m = _mapper.Map<SysLog>(dto);
            _sysLogRepository.Add(m);
            _sysLogRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _sysLogRepository.Remove(recordId);
            _sysLogRepository.SaveChanges();
        }

        public void Update(SysLogDto dto)
        {
            var m = _mapper.Map<SysLog>(dto);
            _sysLogRepository.Update(m);
            _sysLogRepository.SaveChanges();
        }

        public SysLogDto GetById(long recordId)
        {
            var entity = _sysLogRepository.GetById(recordId);
            var dto = _mapper.Map<SysLogDto>(entity);
            return dto;
        }

        public IEnumerable<SysLogDto> GetAll()
        {
            var sysLogs = _sysLogRepository.GetAll();
            return _mapper.Map<IEnumerable<SysLogDto>>(sysLogs);
        }
    }
}
