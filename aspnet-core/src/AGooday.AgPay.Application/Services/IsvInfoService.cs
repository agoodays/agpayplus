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

        public bool Add(IsvInfoDto dto)
        {
            do
            {
                dto.IsvNo = $"V{DateTimeOffset.Now.ToUnixTimeSeconds()}";
            } while (IsExistIsvNo(dto.IsvNo));
            var m = _mapper.Map<IsvInfo>(dto);
            _isvInfoRepository.Add(m);
            return _isvInfoRepository.SaveChanges() > 0;
        }

        public bool Remove(string recordId)
        {
            _isvInfoRepository.Remove(recordId);
            return _isvInfoRepository.SaveChanges() > 0;
        }

        public bool Update(IsvInfoDto dto)
        {
            var m = _mapper.Map<IsvInfo>(dto);
            _isvInfoRepository.Update(m);
            return _isvInfoRepository.SaveChanges() > 0;
        }

        public IsvInfoDto GetById(string recordId)
        {
            var entity = _isvInfoRepository.GetById(recordId);
            var dto = _mapper.Map<IsvInfoDto>(entity);
            return dto;
        }

        public IEnumerable<IsvInfoDto> GetAll()
        {
            var isvInfos = _isvInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<IsvInfoDto>>(isvInfos);
        }

        public PaginatedList<IsvInfoDto> GetPaginatedData(IsvInfoQueryDto dto)
        {
            var isvInfos = _isvInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Contains(dto.IsvName) || w.IsvShortName.Contains(dto.IsvName))
                && (dto.State.Equals(0) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<IsvInfo>.Create<IsvInfoDto>(isvInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
        public bool IsExistIsvNo(string isvNo)
        {
            return _isvInfoRepository.IsExistIsvNo(isvNo);
        }
    }
}
