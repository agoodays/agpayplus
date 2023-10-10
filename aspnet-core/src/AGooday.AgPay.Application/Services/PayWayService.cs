using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付方式表 服务实现类
    /// </summary>
    public class PayWayService : IPayWayService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayWayRepository _payWayRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayWayService(IPayWayRepository payWayRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payWayRepository = payWayRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(PayWayDto dto)
        {
            dto.WayCode = dto.WayCode.ToUpper();
            var m = _mapper.Map<PayWay>(dto);
            _payWayRepository.Add(m);
            return _payWayRepository.SaveChanges(out int _);
        }

        public bool Remove(string recordId)
        {
            _payWayRepository.Remove(recordId);
            return _payWayRepository.SaveChanges(out int _);
        }

        public bool Update(PayWayDto dto)
        {
            var m = _mapper.Map<PayWay>(dto);
            _payWayRepository.Update(m);
            return _payWayRepository.SaveChanges(out int _);
        }

        public PayWayDto GetById(string recordId)
        {
            var entity = _payWayRepository.GetById(recordId);
            var dto = _mapper.Map<PayWayDto>(entity);
            return dto;
        }

        public bool IsExistPayWayCode(string wayCode)
        {
            return _payWayRepository.IsExistPayWayCode(wayCode);
        }

        public IEnumerable<PayWayDto> GetAll()
        {
            var payWays = _payWayRepository.GetAll();
            return _mapper.Map<IEnumerable<PayWayDto>>(payWays);
        }

        public PaginatedList<T> GetPaginatedData<T>(PayWayQueryDto dto)
        {
            var payWays = _payWayRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.WayName) || w.WayName.Contains(dto.WayName))
                && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType))
                ).OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayWay>.Create<T>(payWays.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
