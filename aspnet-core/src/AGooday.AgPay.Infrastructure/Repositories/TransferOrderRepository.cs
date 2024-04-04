using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class TransferOrderRepository : AgPayRepository<TransferOrder>, ITransferOrderRepository
    {
        public TransferOrderRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo)
        {
            return GetAllAsNoTracking().Any(c => c.MchNo.Equals(mchNo) && c.MchOrderNo.Equals(mchOrderNo));
        }
    }
}
