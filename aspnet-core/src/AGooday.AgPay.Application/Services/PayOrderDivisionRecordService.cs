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
    public class PayOrderDivisionRecordService : IPayOrderDivisionRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderDivisionRecordRepository _payOrderDivisionRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderDivisionRecordService(IPayOrderDivisionRecordRepository payOrderDivisionRecordRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payOrderDivisionRecordRepository = payOrderDivisionRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(PayOrderDivisionRecordVM vm)
        {
            var m = _mapper.Map<PayOrderDivisionRecord>(vm);
            _payOrderDivisionRecordRepository.Add(m);
        }

        public void Remove(long recordId)
        {
            _payOrderDivisionRecordRepository.Remove(recordId);
        }

        public void Update(PayOrderDivisionRecordVM vm)
        {
            var m = _mapper.Map<PayOrderDivisionRecord>(vm);
            _payOrderDivisionRecordRepository.Update(m);
        }

        public PayOrderDivisionRecordVM GetById(long recordId)
        {
            var entity = _payOrderDivisionRecordRepository.GetById(recordId);
            var vm = _mapper.Map<PayOrderDivisionRecordVM>(entity);
            return vm;
        }

        public IEnumerable<PayOrderDivisionRecordVM> GetAll()
        {
            var payOrderDivisionRecords = _payOrderDivisionRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderDivisionRecordVM>>(payOrderDivisionRecords);
        }
    }
}
