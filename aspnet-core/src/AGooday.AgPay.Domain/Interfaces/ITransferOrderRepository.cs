using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ITransferOrderRepository : IRepository<TransferOrder>
    {
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
    }
}
