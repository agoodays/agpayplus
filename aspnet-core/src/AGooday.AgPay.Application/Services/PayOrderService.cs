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
    public class PayOrderService : IPayOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderRepository _payOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderService(IPayOrderRepository payOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payOrderRepository = payOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(PayOrderDto dto)
        {
            var m = _mapper.Map<PayOrder>(dto);
            _payOrderRepository.Add(m);
            _payOrderRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _payOrderRepository.Remove(recordId);
            _payOrderRepository.SaveChanges();
        }

        public void Update(PayOrderDto dto)
        {
            var m = _mapper.Map<PayOrder>(dto);
            _payOrderRepository.Update(m);
            _payOrderRepository.SaveChanges();
        }

        public PayOrderDto GetById(string recordId)
        {
            var entity = _payOrderRepository.GetById(recordId);
            var dto = _mapper.Map<PayOrderDto>(entity);
            return dto;
        }

        public IEnumerable<PayOrderDto> GetAll()
        {
            var payOrders = _payOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderDto>>(payOrders);
        }

        /// <summary>
        /// 通用列表查询条件
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PaginatedList<PayOrderDto> GetPaginatedData(PayOrderQueryDto dto)
        {
            var mchInfos = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(0) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.DivisionState.Equals(null) || w.DivisionState.Equals(dto.DivisionState))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrder>.Create<PayOrderDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        public bool IsExistOrderUseIfCode(string ifCode)
        {
            return _payOrderRepository.IsExistOrderUseIfCode(ifCode);
        }

        public bool IsExistOrderUseWayCode(string wayCode)
        {
            return _payOrderRepository.IsExistOrderUseWayCode(wayCode);
        }
    }
}
