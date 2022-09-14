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
    public class PayWayService : IPayWayService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayWayRepository _payWayRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayWayService(IPayWayRepository payWayRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payWayRepository = payWayRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(PayWayVM vm)
        {
            var m = _mapper.Map<PayWay>(vm);
            _payWayRepository.Add(m);
        }

        public void Remove(string recordId)
        {
            _payWayRepository.Remove(recordId);
        }

        public void Update(PayWayVM vm)
        {
            var m = _mapper.Map<PayWay>(vm);
            _payWayRepository.Update(m);
        }

        public PayWayVM GetById(string recordId)
        {
            var entity = _payWayRepository.GetById(recordId);
            var vm = _mapper.Map<PayWayVM>(entity);
            return vm;
        }

        public IEnumerable<PayWayVM> GetAll()
        {
            var payWays = _payWayRepository.GetAll();
            return _mapper.Map<IEnumerable<PayWayVM>>(payWays);
        }
    }
}
