using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class AccountBillRepository : Repository<AccountBill>, IAccountBillRepository
    {
        public AccountBillRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
