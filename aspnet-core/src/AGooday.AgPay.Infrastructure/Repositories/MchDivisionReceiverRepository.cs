using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchDivisionReceiverRepository : AgPayRepository<MchDivisionReceiver, long>, IMchDivisionReceiverRepository
    {
        public MchDivisionReceiverRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistUseReceiverGroupAsync(long receiverGroupId)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.ReceiverGroupId.Equals(receiverGroupId));
        }
    }
}
