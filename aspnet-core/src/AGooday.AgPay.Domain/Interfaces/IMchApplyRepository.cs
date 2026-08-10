using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchApplyRepository : IAgPayRepository<MchApply>
    {
        Task<MchApply> GetByIdAsNoTrackingAsync(string applyId);
        Task<MchApply> GetByIdAsync(string applyId);
        Task<MchApply> GetByIdAsNoTrackingAsync(string applyId, string mchNo);
        Task<bool> IsExistApplyIdAsync(string applyId);
        Task<MchApply> GetByChannelApplyNoAsync(string channelApplyNo);
    }
}
