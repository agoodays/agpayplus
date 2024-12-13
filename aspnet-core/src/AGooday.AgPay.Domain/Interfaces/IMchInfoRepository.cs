using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchInfoRepository : IAgPayRepository<MchInfo>
    {
        Task<bool> IsExistMchNoAsync(string mchNo);
        Task<bool> IsExistMchByIsvNoAsync(string isvNo);
        Task<bool> IsExistMchByAgentNoAsync(string agentNo);
    }
}
