using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchStoreRepository : AgPayRepository<MchStore>, IMchStoreRepository
    {
        public MchStoreRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public async Task<MchStore> GetByIdAsync(long recordId)
        {
            return await GetAll().FirstOrDefaultAsync(w => w.StoreId.Equals(recordId));
        }

        public async Task<MchStore> GetByIdAsNoTrackingAsync(long recordId)
        {
            return await GetAllAsNoTracking().FirstOrDefaultAsync(w => w.StoreId.Equals(recordId));
        }

        public MchStore GetById(long recordId, string mchNo)
        {
            return GetAll().FirstOrDefault(w => w.MchNo.Equals(mchNo) && w.StoreId.Equals(recordId));
        }

        public async Task<MchStore> GetByIdAsync(long recordId, string mchNo)
        {
            return await GetAll().FirstOrDefaultAsync(w => w.MchNo.Equals(mchNo) && w.StoreId.Equals(recordId));
        }

        public async Task<MchStore> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            return await GetAllAsNoTracking().FirstOrDefaultAsync(w => w.MchNo.Equals(mchNo) && w.StoreId.Equals(recordId));
        }
    }
}
