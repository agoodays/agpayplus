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

namespace AGooday.AgPay.Application.Services
{
    public class TransferOrderService : ITransferOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ITransferOrderRepository _transferOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public TransferOrderService(ITransferOrderRepository transferOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _transferOrderRepository = transferOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(TransferOrderDto dto)
        {
            var m = _mapper.Map<TransferOrder>(dto);
            _transferOrderRepository.Add(m);
            _transferOrderRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _transferOrderRepository.Remove(recordId);
            _transferOrderRepository.SaveChanges();
        }

        public void Update(TransferOrderDto dto)
        {
            var m = _mapper.Map<TransferOrder>(dto);
            _transferOrderRepository.Update(m);
            _transferOrderRepository.SaveChanges();
        }

        public TransferOrderDto GetById(string recordId)
        {
            var entity = _transferOrderRepository.GetById(recordId);
            var dto = _mapper.Map<TransferOrderDto>(entity);
            return dto;
        }

        public TransferOrderDto QueryMchOrder(string mchNo, string mchOrderNo, string transferId)
        {
            if (string.IsNullOrEmpty(transferId))
            {
                var entity = _transferOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.TransferId.Equals(transferId)).FirstOrDefault();
                return _mapper.Map<TransferOrderDto>(entity);
            }
            else if (string.IsNullOrEmpty(mchOrderNo))
            {
                var entity = _transferOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.MchOrderNo.Equals(mchOrderNo)).FirstOrDefault();
                return _mapper.Map<TransferOrderDto>(entity);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<TransferOrderDto> GetAll()
        {
            var transferOrders = _transferOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<TransferOrderDto>>(transferOrders);
        }

        public PaginatedList<TransferOrderDto> GetPaginatedData(TransferOrderQueryDto dto)
        {
            var mchInfos = _transferOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(0) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.TransferId) || w.TransferId.Equals(dto.TransferId))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.ChannelOrderNo) || w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.TransferId.Equals(dto.UnionOrderId)
                || w.MchOrderNo.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<TransferOrder>.Create<TransferOrderDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
