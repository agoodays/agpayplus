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
    public class SysEntitlementService : ISysEntitlementService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysEntitlementRepository _sysEntitlementRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysEntitlementService(ISysEntitlementRepository sysEntitlementRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysEntitlementRepository = sysEntitlementRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysEntitlementVM vm)
        {
            var m = _mapper.Map<SysEntitlement>(vm);
            _sysEntitlementRepository.Add(m);
            _sysEntitlementRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysEntitlementRepository.Remove(recordId);
            _sysEntitlementRepository.SaveChanges();
        }

        public void Update(SysEntitlementVM vm)
        {
            var m = _mapper.Map<SysEntitlement>(vm);
            _sysEntitlementRepository.Update(m);
            _sysEntitlementRepository.SaveChanges();
        }

        public SysEntitlementVM GetById(string recordId)
        {
            var entity = _sysEntitlementRepository.GetById(recordId);
            var vm = _mapper.Map<SysEntitlementVM>(entity);
            return vm;
        }

        public IEnumerable<SysEntitlementVM> GetAll()
        {
            var sysEntitlements = _sysEntitlementRepository.GetAll();
            return _mapper.Map<IEnumerable<SysEntitlementVM>>(sysEntitlements);
        }
    }
}
