using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Services
{
    public class MchAppService : IMchAppService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchAppRepository _mchAppRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchAppService(IMchAppRepository mchAppRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchAppRepository = mchAppRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchAppVM vm)
        {
            var m = _mapper.Map<MchApp>(vm);
            _mchAppRepository.Add(m);
            _mchAppRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _mchAppRepository.Remove(recordId);
            _mchAppRepository.SaveChanges();
        }

        public void Update(MchAppVM vm)
        {
            var m = _mapper.Map<MchApp>(vm);
            _mchAppRepository.Update(m);
            _mchAppRepository.SaveChanges();
        }

        public MchAppVM GetById(string recordId)
        {
            var entity = _mchAppRepository.GetById(recordId);
            var vm = _mapper.Map<MchAppVM>(entity);
            return vm;
        }

        public IEnumerable<MchAppVM> GetAll()
        {
            var mchApps = _mchAppRepository.GetAll();
            return _mapper.Map<IEnumerable<MchAppVM>>(mchApps);
        }

        public PaginatedList<MchAppVM> GetPaginatedData(MchAppVM vm, int pageIndex, int pageSize)
        {
            var mchApps = _mchAppRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(vm.MchNo) || w.MchNo.Equals(vm.MchNo))
                && (string.IsNullOrWhiteSpace(vm.AppId) || w.AppId.Equals(vm.AppId))
                && (string.IsNullOrWhiteSpace(vm.AppName) || w.AppName.Contains(vm.AppName))
                && (vm.State.Equals(0) || w.State.Equals(vm.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchApp>.Create<MchAppVM>(mchApps.AsNoTracking(), _mapper, pageIndex, pageSize);
            return records;
        }
    }
}
