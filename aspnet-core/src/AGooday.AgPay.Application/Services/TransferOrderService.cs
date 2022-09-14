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
    public class TransferOrderService : ITransferOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ITransferOrderRepository _transferOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public TransferOrderService(ITransferOrderRepository transferOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _transferOrderRepository = transferOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(TransferOrderVM vm)
        {
            var m = _mapper.Map<TransferOrder>(vm);
            _transferOrderRepository.Add(m);
        }

        public void Remove(string recordId)
        {
            _transferOrderRepository.Remove(recordId);
        }

        public void Update(TransferOrderVM vm)
        {
            var m = _mapper.Map<TransferOrder>(vm);
            _transferOrderRepository.Update(m);
        }

        public TransferOrderVM GetById(string recordId)
        {
            var entity = _transferOrderRepository.GetById(recordId);
            var vm = _mapper.Map<TransferOrderVM>(entity);
            return vm;
        }

        public IEnumerable<TransferOrderVM> GetAll()
        {
            var transferOrders = _transferOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<TransferOrderVM>>(transferOrders);
        }
    }
}
