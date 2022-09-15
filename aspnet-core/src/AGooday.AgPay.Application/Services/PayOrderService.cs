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
    public class PayOrderService : IPayOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderRepository _payOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderService(IPayOrderRepository payOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payOrderRepository = payOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(PayOrderVM vm)
        {
            var m = _mapper.Map<PayOrder>(vm);
            _payOrderRepository.Add(m);
            _payOrderRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _payOrderRepository.Remove(recordId);
            _payOrderRepository.SaveChanges();
        }

        public void Update(PayOrderVM vm)
        {
            var m = _mapper.Map<PayOrder>(vm);
            _payOrderRepository.Update(m);
            _payOrderRepository.SaveChanges();
        }

        public PayOrderVM GetById(string recordId)
        {
            var entity = _payOrderRepository.GetById(recordId);
            var vm = _mapper.Map<PayOrderVM>(entity);
            return vm;
        }

        public IEnumerable<PayOrderVM> GetAll()
        {
            var payOrders = _payOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderVM>>(payOrders);
        }
    }
}
