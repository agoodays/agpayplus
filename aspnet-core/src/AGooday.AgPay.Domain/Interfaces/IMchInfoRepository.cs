using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchInfoRepository : IAgPayRepository<MchInfo>
    {
        bool IsExistMchNo(string mchNo);
        bool IsExistMchByIsvNo(string isvNo);
        bool IsExistMchByAgentNo(string agentNo);
    }
}
