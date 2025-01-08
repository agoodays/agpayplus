using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
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

        public Task<PaginatedList<AccountBillDto>> GetPaginatedDataAsync(AccountBillQueryDto dto)
        {
            var query = _agPayRepository.GetAllAsNoTracking()
                .Where(w => (dto.Id.Equals(null) || w.Id.Equals(dto.Id))
                && (string.IsNullOrWhiteSpace(dto.BillId) || w.BillId.Equals(dto.BillId))
                && (string.IsNullOrWhiteSpace(dto.InfoId) || w.InfoId.Equals(dto.InfoId))
                && (string.IsNullOrWhiteSpace(dto.InfoType) || w.InfoType.Equals(dto.InfoType))
                && (dto.BizType.Equals(null) || w.BizType.Equals(dto.BizType))
                && (dto.AccountType.Equals(null) || w.AccountType.Equals(dto.AccountType))
                && (string.IsNullOrWhiteSpace(dto.RelaBizOrderId) || w.RelaBizOrderId.Equals(dto.RelaBizOrderId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<AccountBill>.CreateAsync<AccountBillDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
