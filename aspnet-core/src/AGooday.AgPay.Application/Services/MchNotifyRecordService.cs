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
using Microsoft.EntityFrameworkCore;
using AGooday.AgPay.Common.Enumerator;

namespace AGooday.AgPay.Application.Services
{
    public class MchNotifyRecordService : IMchNotifyRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchNotifyRecordRepository _mchNotifyRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchNotifyRecordService(IMchNotifyRecordRepository mchNotifyRecordRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchNotifyRecordRepository = mchNotifyRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchNotifyRecordDto dto)
        {
            var m = _mapper.Map<MchNotifyRecord>(dto);
            _mchNotifyRecordRepository.Add(m);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _mchNotifyRecordRepository.Remove(recordId);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public void Update(MchNotifyRecordDto dto)
        {
            var m = _mapper.Map<MchNotifyRecord>(dto);
            _mchNotifyRecordRepository.Update(m);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public MchNotifyRecordDto GetById(long recordId)
        {
            var entity = _mchNotifyRecordRepository.GetById(recordId);
            var dto = _mapper.Map<MchNotifyRecordDto>(entity);
            return dto;
        }

        public IEnumerable<MchNotifyRecordDto> GetAll()
        {
            var mchNotifyRecords = _mchNotifyRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<MchNotifyRecordDto>>(mchNotifyRecords);
        }
        public PaginatedList<MchNotifyRecordDto> GetPaginatedData(MchNotifyQueryDto dto)
        {
            var mchInfos = _mchNotifyRecordRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.OrderId) || w.OrderId.Equals(dto.OrderId))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.OrderType.Equals(0) || w.OrderType.Equals(dto.OrderType))
                && (dto.State.Equals(0) || w.State.Equals(dto.State))
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchNotifyRecord>.Create<MchNotifyRecordDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
        public void UpdateIngAndAddNotifyCountLimit(long notifyId)
        {
            var notify = _mchNotifyRecordRepository.GetById(notifyId);
            notify.NotifyCountLimit += 1;
            notify.State = (byte)MchNotifyRecordState.STATE_ING;
            _mchNotifyRecordRepository.Update(notify);
            _mchNotifyRecordRepository.SaveChanges();
        }
    }
}
