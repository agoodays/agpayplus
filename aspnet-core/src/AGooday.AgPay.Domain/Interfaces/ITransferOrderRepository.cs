using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ITransferOrderRepository : IAgPayRepository<TransferOrder>
    {
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
    }
}
