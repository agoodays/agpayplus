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
    public class PayInterfaceDefineService : IPayInterfaceDefineService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayInterfaceDefineService(IPayInterfaceDefineRepository payInterfaceDefineRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(PayInterfaceDefineDto dto)
        {
            var m = _mapper.Map<PayInterfaceDefine>(dto);
            _payInterfaceDefineRepository.Add(m);
            return _payInterfaceDefineRepository.SaveChanges(out int _);
        }

        public bool Remove(string recordId)
        {
            _payInterfaceDefineRepository.Remove(recordId);
            return _payInterfaceDefineRepository.SaveChanges(out int _);
        }

        public bool Update(PayInterfaceDefineDto dto)
        {
            var renew = _mapper.Map<PayInterfaceDefine>(dto);
            //var old = _payInterfaceDefineRepository.GetById(dto.IfCode);
            renew.UpdatedAt = DateTime.Now;
            _payInterfaceDefineRepository.Update(renew);
            return _payInterfaceDefineRepository.SaveChanges(out int _);
        }

        public PayInterfaceDefineDto GetById(string recordId)
        {
            var entity = _payInterfaceDefineRepository.GetById(recordId);
            var dto = _mapper.Map<PayInterfaceDefineDto>(entity);
            return dto;
        }

        public IEnumerable<PayInterfaceDefineDto> GetAll()
        {
            var payInterfaceDefines = _payInterfaceDefineRepository.GetAll();
            return _mapper.Map<IEnumerable<PayInterfaceDefineDto>>(payInterfaceDefines);
        }
    }
}
