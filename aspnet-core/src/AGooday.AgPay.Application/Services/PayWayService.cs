using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付方式表 服务实现类
    /// </summary>
    public class PayWayService : AgPayService<PayWayDto, PayWay>, IPayWayService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayWayRepository _payWayRepository;

        public PayWayService(IMapper mapper, IMediatorHandler bus,
            IPayWayRepository payWayRepository)
            : base(mapper, bus, payWayRepository)
        {
            _payWayRepository = payWayRepository;
        }

        public Task<bool> IsExistPayWayCodeAsync(string wayCode)
        {
            return _payWayRepository.IsExistPayWayCodeAsync(wayCode);
        }

        public override async Task<bool> AddAsync(PayWayDto dto)
        {
            dto.WayCode = dto.WayCode.ToUpper();
            var entity = _mapper.Map<PayWay>(dto);
            await _payWayRepository.AddAsync(entity);
            return await _payWayRepository.SaveChangesAsync() > 0;
        }

        public async Task<string> GetWayTypeByWayCodeAsync(string wayCode)
        {
            var entity = await _payWayRepository.GetByIdAsync(wayCode);
            return entity?.WayType ?? PayWayType.OTHER.ToString();
        }

        public Task<PaginatedList<T>> GetPaginatedDataAsync<T>(PayWayQueryDto dto)
        {
            var query = _payWayRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.WayName) || w.WayName.Contains(dto.WayName))
                && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType)))
                .OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayWay>.CreateAsync<T>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
