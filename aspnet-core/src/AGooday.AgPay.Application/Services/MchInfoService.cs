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
    public class MchInfoService: IMchInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchInfoRepository _mchInfoRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchInfoService(IMchInfoRepository mchInfoRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchInfoRepository = mchInfoRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchInfoVM vm)
        {
            var m = _mapper.Map<MchInfo>(vm);
            _mchInfoRepository.Add(m);
        }

        public void Remove(string recordId)
        {
            _mchInfoRepository.Remove(recordId);
        }

        public void Update(MchInfoVM vm)
        {
            var m = _mapper.Map<MchInfo>(vm);
            _mchInfoRepository.Update(m);
        }

        public MchInfoVM GetById(string recordId)
        {
            var entity = _mchInfoRepository.GetById(recordId);
            var vm = _mapper.Map<MchInfoVM>(entity);
            return vm;
        }

        public IEnumerable<MchInfoVM> GetAll()
        {
            var mchInfos = _mchInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<MchInfoVM>>(mchInfos);
        }
    }
}
