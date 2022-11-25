using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchPayPassageRepository : IRepository<MchPayPassage, long>
    {
        bool IsExistMchPayPassageUseWayCode(string wayCode);
        void RemoveByMchNo(string mchNo);
    }
}
