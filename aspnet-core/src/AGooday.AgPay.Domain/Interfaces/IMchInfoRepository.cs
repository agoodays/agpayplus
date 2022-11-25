using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchInfoRepository : IRepository<MchInfo>
    {
        bool IsExistMchNo(string mchNo);
    }
}
