using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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
            return DbSet.AsNoTracking().Any(c => c.MchNo.Equals(mchNo) && c.MchOrderNo.Equals(mchOrderNo));
        }
    }
}
