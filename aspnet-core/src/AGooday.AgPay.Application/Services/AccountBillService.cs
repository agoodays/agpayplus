using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
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
    public class AccountBillService : AgPayService<AccountBillDto, AccountBill>, IAccountBillService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderProfitRepository _payOrderProfitRepository;

        public AccountBillService(IMapper mapper, IMediatorHandler bus,
            IAccountBillRepository accountBillRepository,
            IPayOrderProfitRepository payOrderProfitRepository)
            : base(mapper, bus, accountBillRepository)
        {
            _payOrderProfitRepository = payOrderProfitRepository;
        }

        public async Task GenAccountBillAsync(string payOrderId)
        {
            var accountBills = _payOrderProfitRepository.GetByPayOrderIdAsNoTracking(payOrderId)
                .OrderBy(o => o.Id)
                .Where(w => w.ProfitAmount > 0)
                .Select(s => new AccountBill
                {
                    BillId = SeqUtil.GenBillId(),
                    InfoId = s.InfoId,
                    InfoName = s.InfoName,
                    InfoType = s.InfoType,
                    BeforeBalance = 0,
                    ChangeAmount = s.ProfitAmount,
                    AfterBalance = s.ProfitAmount,
                    BizType = (byte)AccountBillBizType.ORDER_PROFIT_CALCULATE,
                    AccountType = (byte)AccountBillAccountType.IN_TRANSIT_ACCOUNT,
                    RelaBizOrderType = (byte)AccountBillRelaBizOrderType.PAY_ORDER,
                    RelaBizOrderId = s.PayOrderId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            if (accountBills.Any())
            {
                await _agPayRepository.AddRangeAsync(accountBills);
                await _agPayRepository.SaveChangesAsync();
            }
        }

        public override async Task<bool> AddAsync(AccountBillDto dto)
        {
            var entity = _mapper.Map<AccountBill>(dto);
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            await _agPayRepository.AddAsync(entity);
            var (result, _) = await _agPayRepository.SaveChangesWithResultAsync();
            return result;
        }

        public override async Task<bool> UpdateAsync(AccountBillDto dto)
        {
            var entity = _mapper.Map<AccountBill>(dto);
            entity.UpdatedAt = DateTime.Now;
            _agPayRepository.Update(entity);
            var (result, _) = await _agPayRepository.SaveChangesWithResultAsync();
            return result;
        }

        public Task<PaginatedResult<AccountBillDto>> GetPaginatedDataAsync(AccountBillQueryDto dto)
        {
            var query = _agPayRepository.GetAllAsNoTracking()
                .WhereIfNotNull(dto.Id, w => w.Id.Equals(dto.Id))
                .WhereIfNotEmpty(dto.BillId, w => w.BillId.Equals(dto.BillId))
                .WhereIfNotEmpty(dto.InfoId, w => w.InfoId.Equals(dto.InfoId))
                .WhereIfNotEmpty(dto.InfoType, w => w.InfoType.Equals(dto.InfoType))
                .WhereIfNotNull(dto.BizType, w => w.BizType.Equals(dto.BizType))
                .WhereIfNotNull(dto.AccountType, w => w.AccountType.Equals(dto.AccountType))
                .WhereIfNotEmpty(dto.RelaBizOrderId, w => w.RelaBizOrderId.Equals(dto.RelaBizOrderId))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd)
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<AccountBill, AccountBillDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
