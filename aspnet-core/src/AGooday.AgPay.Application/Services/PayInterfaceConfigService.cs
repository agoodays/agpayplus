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
    public class PayInterfaceConfigService : IPayInterfaceConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayInterfaceConfigService(IPayInterfaceConfigRepository payInterfaceConfigRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payInterfaceConfigRepository = payInterfaceConfigRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(PayInterfaceConfigVM vm)
        {
            var m = _mapper.Map<PayInterfaceConfig>(vm);
            _payInterfaceConfigRepository.Add(m);
        }

        public void Remove(long recordId)
        {
            _payInterfaceConfigRepository.Remove(recordId);
        }

        public void Update(PayInterfaceConfigVM vm)
        {
            var m = _mapper.Map<PayInterfaceConfig>(vm);
            _payInterfaceConfigRepository.Update(m);
        }

        public PayInterfaceConfigVM GetById(long recordId)
        {
            var entity = _payInterfaceConfigRepository.GetById(recordId);
            var vm = _mapper.Map<PayInterfaceConfigVM>(entity);
            return vm;
        }

        public IEnumerable<PayInterfaceConfigVM> GetAll()
        {
            var payInterfaceConfigs = _payInterfaceConfigRepository.GetAll();
            return _mapper.Map<IEnumerable<PayInterfaceConfigVM>>(payInterfaceConfigs);
        }
    }
}
