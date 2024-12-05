using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付接口定义表 服务实现类
    /// </summary>
    public class PayInterfaceDefineService : AgPayService<PayInterfaceDefineDto, PayInterfaceDefine>, IPayInterfaceDefineService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;

        public PayInterfaceDefineService(IMapper mapper, IMediatorHandler bus,
            IPayInterfaceDefineRepository payInterfaceDefineRepository)
            : base(mapper, bus, payInterfaceDefineRepository)
        {
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
        }

        public override bool Add(PayInterfaceDefineDto dto)
        {
            var entity = _mapper.Map<PayInterfaceDefine>(dto);
            dto.CreatedAt = DateTime.Now;
            dto.UpdatedAt = DateTime.Now;
            _payInterfaceDefineRepository.Add(entity);
            return _payInterfaceDefineRepository.SaveChanges(out int _);
        }

        public override bool Update(PayInterfaceDefineDto dto)
        {
            var entity = _mapper.Map<PayInterfaceDefine>(dto);
            entity.UpdatedAt = DateTime.Now;
            _payInterfaceDefineRepository.Update(entity);
            return _payInterfaceDefineRepository.SaveChanges(out int _);
        }

        public IEnumerable<PayInterfaceDefineDto> GetByIfCodes(IEnumerable<string> ifCodes)
        {
            var entitys = _payInterfaceDefineRepository.GetAll()
                .Where(w => ifCodes.Contains(w.IfCode)); ;
            var result = _mapper.Map<IEnumerable<PayInterfaceDefineDto>>(entitys);
            return result;
        }
    }
}
