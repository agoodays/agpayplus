using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderRepository : IAgPayRepository<PayOrder>
    {
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
        bool IsExistOrderUseIfCode(string ifCode);
        bool IsExistOrderUseMchNo(string mchNo);
        bool IsExistOrderUseWayCode(string wayCode);
    }
}
