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

        public override async Task<bool> AddAsync(PayInterfaceDefineDto dto)
        {
            var entity = _mapper.Map<PayInterfaceDefine>(dto);
            dto.CreatedAt = DateTime.Now;
            dto.UpdatedAt = DateTime.Now;
            await _payInterfaceDefineRepository.AddAsync(entity);
            var (result, _) = await _payInterfaceDefineRepository.SaveChangesWithResultAsync();
            return result;
        }

        public override async Task<bool> UpdateAsync(PayInterfaceDefineDto dto)
        {
            var entity = _mapper.Map<PayInterfaceDefine>(dto);
            entity.UpdatedAt = DateTime.Now;
            _payInterfaceDefineRepository.Update(entity);
            var (result, _) = await _payInterfaceDefineRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<IEnumerable<PayInterfaceDefineDto>> PayIfDefineListAsync(byte? state)
        {
            var query = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .WhereIfNotNull(state, w => w.State.Equals(state))
                .OrderByDescending(o => o.CreatedAt);

            return await query.ToListAsync<PayInterfaceDefine, PayInterfaceDefineDto>(_mapper);
        }

        public async Task<IEnumerable<PayInterfaceDefineDto>> GetByIfCodesAsync(IEnumerable<string> ifCodes)
        {
            var query = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => ifCodes.Contains(w.IfCode));

            return await query.ToListAsync<PayInterfaceDefine, PayInterfaceDefineDto>(_mapper);
        }
    }
}
