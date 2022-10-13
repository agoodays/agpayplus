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
using System.Data;

namespace AGooday.AgPay.Application.Services
{
    public class RefundOrderService : IRefundOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IRefundOrderRepository _refundOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public RefundOrderService(IRefundOrderRepository refundOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _refundOrderRepository = refundOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(RefundOrderDto dto)
        {
            var m = _mapper.Map<RefundOrder>(dto);
            _refundOrderRepository.Add(m);
            _refundOrderRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _refundOrderRepository.Remove(recordId);
            _refundOrderRepository.SaveChanges();
        }

        public void Update(RefundOrderDto dto)
        {
            var m = _mapper.Map<RefundOrder>(dto);
            _refundOrderRepository.Update(m);
            _refundOrderRepository.SaveChanges();
        }

        public RefundOrderDto GetById(string recordId)
        {
            var entity = _refundOrderRepository.GetById(recordId);
            var dto = _mapper.Map<RefundOrderDto>(entity);
            return dto;
        }

        public IEnumerable<RefundOrderDto> GetAll()
        {
            var refundOrders = _refundOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<RefundOrderDto>>(refundOrders);
        }

        public PaginatedList<RefundOrderDto> GetPaginatedData(RefundOrderQueryDto dto)
        {
            var mchInfos = _refundOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(0) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.RefundOrderId) || w.RefundOrderId.Equals(dto.RefundOrderId))
                && (string.IsNullOrWhiteSpace(dto.MchRefundNo) || w.MchRefundNo.Equals(dto.MchRefundNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId)
                || w.RefundOrderId.Equals(dto.UnionOrderId) || w.MchRefundNo.Equals(dto.UnionOrderId)
                || w.ChannelPayOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<RefundOrder>.Create<RefundOrderDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
        public long SumSuccessRefundAmount(string payOrderId)
        {
            return _refundOrderRepository.GetAll().Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).Sum(s => s.RefundAmount);
        }
        public bool UpdateInit2Ing(string refundOrderId)
        {
            var updateRecord = _refundOrderRepository.GetById(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_INIT)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_ING;
            _refundOrderRepository.Update(updateRecord);
            return _refundOrderRepository.SaveChanges(out int _);
        }

        public bool UpdateIng2Success(string refundOrderId, string channelOrderNo)
        {
            var updateRecord = _refundOrderRepository.GetById(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_ING)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _refundOrderRepository.Update(updateRecord);
            return _refundOrderRepository.SaveChanges(out int _);
        }

        public bool UpdateIng2Fail(string refundOrderId, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = _refundOrderRepository.GetById(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_ING)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _refundOrderRepository.Update(updateRecord);
            return _refundOrderRepository.SaveChanges(out int _);
        }

        public bool UpdateIng2SuccessOrFail(string refundOrderId, byte updateState, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)RefundOrderState.STATE_ING)
            {
                return true;
            }
            else if (updateState == (byte)RefundOrderState.STATE_SUCCESS)
            {
                return UpdateIng2Success(refundOrderId, channelOrderNo);
            }
            else if (updateState == (byte)RefundOrderState.STATE_FAIL)
            {
                return UpdateIng2Fail(refundOrderId, channelOrderNo, channelErrCode, channelErrMsg);
            }
            return false;

        }
        public bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo)
        {
            return _refundOrderRepository.IsExistOrderByMchOrderNo(mchNo, mchRefundNo);
        }
        public bool IsExistRefundingOrder(string payOrderId)
        {
            return _refundOrderRepository.IsExistRefundingOrder(payOrderId);
        }
    }
}
