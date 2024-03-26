using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 账户帐单表 服务实现类
    /// </summary>
    public class PayOrderProfitService : IPayOrderProfitService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderProfitRepository _payOrderProfitRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderProfitService(IMapper mapper, IMediatorHandler bus,
            IPayOrderProfitRepository payOrderProfitRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payOrderProfitRepository = payOrderProfitRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(PayOrderProfitDto dto)
        {
            var m = _mapper.Map<PayOrderProfit>(dto);
            m.CreatedAt = DateTime.Now;
            m.UpdatedAt = DateTime.Now;
            _payOrderProfitRepository.Add(m);
            var result = _payOrderProfitRepository.SaveChanges(out int _);
            return result;
        }

        public bool Remove(long recordId)
        {
            _payOrderProfitRepository.Remove(recordId);
            return _payOrderProfitRepository.SaveChanges(out int _);
        }

        public bool Update(PayOrderProfitDto dto)
        {
            var m = _mapper.Map<PayOrderProfit>(dto);
            m.UpdatedAt = DateTime.Now;
            _payOrderProfitRepository.Update(m);
            return _payOrderProfitRepository.SaveChanges(out int _);
        }

        public PayOrderProfitDto GetById(long recordId)
        {
            var entity = _payOrderProfitRepository.GetById(recordId);
            var dto = _mapper.Map<PayOrderProfitDto>(entity);
            return dto;
        }

        public IEnumerable<PayOrderProfitDto> GetAll()
        {
            var payOrderProfits = _payOrderProfitRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderProfitDto>>(payOrderProfits);
        }

        public IEnumerable<PayOrderProfitDto> GetByPayOrderIdAsNoTracking(string payOrderId)
        {
            var payOrderProfits = _payOrderProfitRepository.GetByPayOrderIdAsNoTracking(payOrderId);
            return _mapper.Map<IEnumerable<PayOrderProfitDto>>(payOrderProfits);
        }
    }
}
