using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchAppRepository : AgPayRepository<MchApp>, IMchAppRepository
    {
        public MchAppRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<MchApp> GetByIdAsNoTrackingAsync(string recordId)
        {
            return GetAllAsNoTracking().FirstOrDefaultAsync(w => w.AppId.Equals(recordId));
        }

        public Task<MchApp> GetByIdAsync(string recordId, string mchNo)
        {
            return GetAll().FirstOrDefaultAsync(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(recordId));
        }

        public Task<MchApp> GetByIdAsNoTrackingAsync(string recordId, string mchNo)
        {
            return GetAllAsNoTracking().FirstOrDefaultAsync(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(recordId));
        }
    }
}
