using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayOrderDivisionRecordRepository : AgPayRepository<PayOrderDivisionRecord, long>, IPayOrderDivisionRecordRepository
    {
        public PayOrderDivisionRecordRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public async Task<PayOrderDivisionRecord> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            return await GetAllAsNoTracking().FirstOrDefaultAsync(w => w.RecordId.Equals(recordId) && w.MchNo.Equals(mchNo));
        }

        public Task<long> SumSuccessDivisionAmountAsync(string payOrderId)
        {
            return DbSet.Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_SUCCESS))
                .SumAsync(s => s.CalDivisionAmount);
        }
    }
}
