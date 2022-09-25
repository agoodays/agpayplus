using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
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
using AGooday.AgPay.Domain.Commands.MchInfos;

namespace AGooday.AgPay.Application.Services
{
    public class MchInfoService : IMchInfoService
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

        public void Add(MchInfoDto dto)
        {
            var m = _mapper.Map<MchInfo>(dto);
            _mchInfoRepository.Add(m);
            _mchInfoRepository.SaveChanges();
        }

        public void Create(MchInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateMchInfoCommand>(dto);
            Bus.SendCommand(command);
        }

        public void Remove(string recordId)
        {
            _mchInfoRepository.Remove(recordId);
            _mchInfoRepository.SaveChanges();
        }

        public void Update(MchInfoDto dto)
        {
            var m = _mapper.Map<MchInfo>(dto);
            _mchInfoRepository.Update(m);
            _mchInfoRepository.SaveChanges();
        }

        public MchInfoDto GetById(string recordId)
        {
            var entity = _mchInfoRepository.GetById(recordId);
            var dto = _mapper.Map<MchInfoDto>(entity);
            return dto;
        }

        public IEnumerable<MchInfoDto> GetAll()
        {
            var mchInfos = _mchInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto)
        {
            var mchInfos = _mchInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (dto.Type.Equals(0) || w.Type.Equals(dto.Type))
                && (dto.State.Equals(0) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchInfo>.Create<MchInfoDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
