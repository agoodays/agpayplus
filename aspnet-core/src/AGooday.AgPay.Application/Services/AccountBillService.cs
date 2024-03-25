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
    public class AccountBillService : IAccountBillService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IAccountBillRepository _accountBillRepository;
        private readonly IPayOrderProfitRepository _payOrderProfitRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public AccountBillService(IMapper mapper, IMediatorHandler bus,
            IAccountBillRepository accountBillRepository,
            IPayOrderProfitRepository payOrderProfitRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _accountBillRepository = accountBillRepository;
            _payOrderProfitRepository = payOrderProfitRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void GenAccountBill(string payOrderId)
        {
            var payOrderProfits = _payOrderProfitRepository.GetByPayOrderId(payOrderId).OrderBy(o => o.Id);
            foreach (var payOrderProfit in payOrderProfits)
            {
                if (payOrderProfit.ProfitAmount > 0)
                {
                    var accountBill = new AccountBill();
                    accountBill.BillId = SeqUtil.GenBillId();
                    accountBill.InfoId = payOrderProfit.InfoId;
                    accountBill.InfoName = payOrderProfit.InfoName;
                    accountBill.InfoType = payOrderProfit.InfoType;
                    accountBill.BeforeBalance = 0;
                    accountBill.ChangeAmount = payOrderProfit.ProfitAmount;
                    accountBill.AfterBalance = payOrderProfit.ProfitAmount;
                    accountBill.BizType = (byte)AccountBillBizType.ORDER_PROFIT_CALCULATE;
                    accountBill.AccountType = (byte)AccountBillAccountType.IN_TRANSIT_ACCOUNT;
                    accountBill.RelaBizOrderType = (byte)AccountBillRelaBizOrderType.PAY_ORDER;
                    accountBill.RelaBizOrderId = payOrderProfit.PayOrderId;
                }
            }
        }

        public bool Add(AccountBillDto dto)
        {
            var m = _mapper.Map<AccountBill>(dto);
            m.CreatedAt = DateTime.Now;
            m.UpdatedAt = DateTime.Now;
            _accountBillRepository.Add(m);
            var result = _accountBillRepository.SaveChanges(out int _);
            return result;
        }

        public bool Remove(string recordId)
        {
            _accountBillRepository.Remove(recordId);
            return _accountBillRepository.SaveChanges(out int _);
        }

        public bool Update(AccountBillDto dto)
        {
            var m = _mapper.Map<AccountBill>(dto);
            m.UpdatedAt = DateTime.Now;
            _accountBillRepository.Update(m);
            return _accountBillRepository.SaveChanges(out int _);
        }

        public AccountBillDto GetById(string recordId)
        {
            var entity = _accountBillRepository.GetById(recordId);
            var dto = _mapper.Map<AccountBillDto>(entity);
            return dto;
        }

        public IEnumerable<AccountBillDto> GetAll()
        {
            var AccountBills = _accountBillRepository.GetAll();
            return _mapper.Map<IEnumerable<AccountBillDto>>(AccountBills);
        }

        public PaginatedList<T> GetPaginatedData<T>(AccountBillQueryDto dto)
        {
            var AccountBills = _accountBillRepository.GetAllAsNoTracking()
                .Where(w => (dto.Id.Equals(null) || w.Id.Equals(dto.Id))
                && (string.IsNullOrWhiteSpace(dto.BillId) || w.BillId.Equals(dto.BillId))
                && (string.IsNullOrWhiteSpace(dto.InfoId) || w.InfoId.Equals(dto.InfoId))
                && (string.IsNullOrWhiteSpace(dto.InfoType) || w.InfoType.Equals(dto.InfoType))
                && (dto.BizType.Equals(null) || w.BizType.Equals(dto.BizType))
                && (dto.AccountType.Equals(null) || w.AccountType.Equals(dto.AccountType))
                && (string.IsNullOrWhiteSpace(dto.RelaBizOrderId) || w.RelaBizOrderId.Equals(dto.RelaBizOrderId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<AccountBill>.Create<T>(AccountBills, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
