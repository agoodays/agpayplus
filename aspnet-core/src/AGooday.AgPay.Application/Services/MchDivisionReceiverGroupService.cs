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
    public class MchDivisionReceiverGroupService: IMchDivisionReceiverGroupService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchDivisionReceiverGroupRepository _mchDivisionReceiverGroupRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchDivisionReceiverGroupService(IMchDivisionReceiverGroupRepository mchDivisionReceiverGroupRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchDivisionReceiverGroupRepository = mchDivisionReceiverGroupRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchDivisionReceiverGroupVM vm)
        {
            var m = _mapper.Map<MchDivisionReceiverGroup>(vm);
            _mchDivisionReceiverGroupRepository.Add(m);
        }

        public void Remove(long recordId)
        {
            _mchDivisionReceiverGroupRepository.Remove(recordId);
        }

        public void Update(MchDivisionReceiverGroupVM vm)
        {
            var m = _mapper.Map<MchDivisionReceiverGroup>(vm);
            _mchDivisionReceiverGroupRepository.Update(m);
        }

        public MchDivisionReceiverGroupVM GetById(long recordId)
        {
            var entity = _mchDivisionReceiverGroupRepository.GetById(recordId);
            var vm = _mapper.Map<MchDivisionReceiverGroupVM>(entity);
            return vm;
        }

        public IEnumerable<MchDivisionReceiverGroupVM> GetAll()
        {
            var mchDivisionReceiverGroups = _mchDivisionReceiverGroupRepository.GetAll();
            return _mapper.Map<IEnumerable<MchDivisionReceiverGroupVM>>(mchDivisionReceiverGroups);
        }
    }
}
