using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
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
            var (result, _) = await _payWayRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<string> GetWayTypeByWayCodeAsync(string wayCode)
        {
            var entity = await _payWayRepository.GetByIdAsync(wayCode);
            return entity?.WayType ?? PayWayType.OTHER.ToString();
        }

        public Task<PaginatedResult<T>> GetPaginatedDataAsync<T>(PayWayQueryDto dto)
        {
            var query = _payWayRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.WayCode, w => w.WayCode.Equals(dto.WayCode))
                .WhereIfNotEmpty(dto.WayName, w => w.WayName.Contains(dto.WayName))
                .WhereIfNotEmpty(dto.WayType, w => w.WayType.Equals(dto.WayType))
                .OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<PayWay, T>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
