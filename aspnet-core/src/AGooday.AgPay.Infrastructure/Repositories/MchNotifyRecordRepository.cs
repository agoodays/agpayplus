using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchNotifyRecordRepository : Repository<MchNotifyRecord, long>, IMchNotifyRecordRepository
    {
        public MchNotifyRecordRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
