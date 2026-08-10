using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchApplyRepository : AgPayRepository<MchApply>, IMchApplyRepository
    {
        public MchApplyRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public async Task<MchApply> GetByIdAsNoTrackingAsync(string applyId)
        {
            return await GetAllAsNoTracking().FirstOrDefaultAsync(w => w.ApplyId.Equals(applyId));
        }

        public async Task<MchApply> GetByIdAsync(string applyId)
        {
            return await GetAll().FirstOrDefaultAsync(w => w.ApplyId.Equals(applyId));
        }

        public async Task<MchApply> GetByIdAsNoTrackingAsync(string applyId, string mchNo)
        {
            return await GetAllAsNoTracking().FirstOrDefaultAsync(w => w.ApplyId.Equals(applyId) && w.MchNo.Equals(mchNo));
        }

        public async Task<bool> IsExistApplyIdAsync(string applyId)
        {
            return await GetAllAsNoTracking().AnyAsync(w => w.ApplyId.Equals(applyId));
        }

        public async Task<MchApply> GetByChannelApplyNoAsync(string channelApplyNo)
        {
            if (string.IsNullOrEmpty(channelApplyNo)) return null;
            return await GetAllAsNoTracking()
                .FirstOrDefaultAsync(w => w.ChannelApplyNo != null && w.ChannelApplyNo.Equals(channelApplyNo));
        }
    }
}
