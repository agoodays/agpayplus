using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
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
    public class MchPayPassageService : IMchPayPassageService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchPayPassageRepository _mchPayPassageRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchPayPassageService(IMchPayPassageRepository mchPayPassageRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchPayPassageRepository = mchPayPassageRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchPayPassageDto dto)
        {
            var m = _mapper.Map<MchPayPassage>(dto);
            _mchPayPassageRepository.Add(m);
            _mchPayPassageRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _mchPayPassageRepository.Remove(recordId);
            _mchPayPassageRepository.SaveChanges();
        }

        public void Update(MchPayPassageDto dto)
        {
            var m = _mapper.Map<MchPayPassage>(dto);
            _mchPayPassageRepository.Update(m);
            _mchPayPassageRepository.SaveChanges();
        }

        public MchPayPassageDto GetById(long recordId)
        {
            var entity = _mchPayPassageRepository.GetById(recordId);
            var dto = _mapper.Map<MchPayPassageDto>(entity);
            return dto;
        }

        public IEnumerable<MchPayPassageDto> GetAll()
        {
            var mchPayPassages = _mchPayPassageRepository.GetAll();
            return _mapper.Map<IEnumerable<MchPayPassageDto>>(mchPayPassages);
        }
    }
}
