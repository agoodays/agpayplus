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

        public override bool Add(PayWayDto dto)
        {
            dto.WayCode = dto.WayCode.ToUpper();
            var m = _mapper.Map<PayWay>(dto);
            _payWayRepository.Add(m);
            return _payWayRepository.SaveChanges(out int _);
        }

        public string GetWayTypeByWayCode(string wayCode)
        {
            var entity = _payWayRepository.GetById(wayCode);
            return entity?.WayType ?? PayWayType.OTHER.ToString();
        }

        public bool IsExistPayWayCode(string wayCode)
        {
            return _payWayRepository.IsExistPayWayCode(wayCode);
        }

        public PaginatedList<T> GetPaginatedData<T>(PayWayQueryDto dto)
        {
            var payWays = _payWayRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.WayName) || w.WayName.Contains(dto.WayName))
                && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType))
                ).OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayWay>.Create<T>(payWays, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
