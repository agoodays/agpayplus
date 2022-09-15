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
    public class IsvInfoService : IIsvInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IIsvInfoRepository _isvInfoRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public IsvInfoService(IIsvInfoRepository isvInfoRepository, IMapper mapper, IMediatorHandler bus)
        {
            _isvInfoRepository = isvInfoRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(IsvInfoVM vm)
        {
            var m = _mapper.Map<IsvInfo>(vm);
            _isvInfoRepository.Add(m);
            _isvInfoRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _isvInfoRepository.Remove(recordId);
            _isvInfoRepository.SaveChanges();
        }

        public void Update(IsvInfoVM vm)
        {
            var m = _mapper.Map<IsvInfo>(vm);
            _isvInfoRepository.Update(m);
            _isvInfoRepository.SaveChanges();
        }

        public IsvInfoVM GetById(string recordId)
        {
            var entity = _isvInfoRepository.GetById(recordId);
            var vm = _mapper.Map<IsvInfoVM>(entity);
            return vm;
        }

        public IEnumerable<IsvInfoVM> GetAll()
        {
            var isvInfos = _isvInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<IsvInfoVM>>(isvInfos);
        }
    }
}
