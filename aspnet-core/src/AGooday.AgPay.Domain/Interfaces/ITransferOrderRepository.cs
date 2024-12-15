using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ITransferOrderRepository : IAgPayRepository<TransferOrder>
    {
        Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchOrderNo);
    }
}
