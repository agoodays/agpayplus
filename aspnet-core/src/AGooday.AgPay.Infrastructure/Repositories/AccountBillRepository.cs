using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class AccountBillRepository : AgPayRepository<AccountBill>, IAccountBillRepository
    {
        public AccountBillRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
