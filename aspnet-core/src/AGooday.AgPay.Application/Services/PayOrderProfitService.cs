using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 账户帐单表 服务实现类
    /// </summary>
    public class PayOrderProfitService : AgPayService<PayOrderProfitDto, PayOrderProfit>, IPayOrderProfitService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderProfitRepository _payOrderProfitRepository;

        public PayOrderProfitService(IMapper mapper, IMediatorHandler bus,
            IPayOrderProfitRepository payOrderProfitRepository)
            : base(mapper, bus, payOrderProfitRepository)
        {
            _payOrderProfitRepository = payOrderProfitRepository;
        }

        public override async Task<bool> AddAsync(PayOrderProfitDto dto)
        {
            var entity = _mapper.Map<PayOrderProfit>(dto);
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            await _payOrderProfitRepository.AddAsync(entity);
            var (result, _) = await _payOrderProfitRepository.SaveChangesWithResultAsync();
            dto.Id = entity.Id;
            return result;
        }

        public override async Task<bool> UpdateAsync(PayOrderProfitDto dto)
        {
            var entity = _mapper.Map<PayOrderProfit>(dto);
            entity.UpdatedAt = DateTime.Now;
            _payOrderProfitRepository.Update(entity);
            var (result, _) = await _payOrderProfitRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<IEnumerable<PayOrderProfitDto>> GetByPayOrderIdAsNoTrackingAsync(string payOrderId)
        {
            var query = _payOrderProfitRepository.GetByPayOrderIdAsNoTracking(payOrderId);
            return await query.ToListProjectToAsync<PayOrderProfit, PayOrderProfitDto>(_mapper);
        }

        public async Task<IEnumerable<PayOrderProfitDto>> GetByPayOrderIdsAsNoTrackingAsync(List<string> payOrderIds)
        {
            var query = _payOrderProfitRepository.GetByPayOrderIdsAsNoTracking(payOrderIds);
            return await query.ToListProjectToAsync<PayOrderProfit, PayOrderProfitDto>(_mapper);
        }
    }
}
