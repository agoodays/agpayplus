using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchDivisionReceiverGroupRepository : Repository<MchDivisionReceiverGroup, long>, IMchDivisionReceiverGroupRepository
    {
        public MchDivisionReceiverGroupRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
