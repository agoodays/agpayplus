using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchDivisionReceiverRepository : AgPayRepository<MchDivisionReceiver, long>, IMchDivisionReceiverRepository
    {
        public MchDivisionReceiverRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistUseReceiverGroup(long receiverGroupId)
        {
            return GetAllAsNoTracking().Any(c => c.ReceiverGroupId.Equals(receiverGroupId));
        }
    }
}
