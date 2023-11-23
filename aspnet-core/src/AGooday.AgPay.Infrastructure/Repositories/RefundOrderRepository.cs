using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class RefundOrderRepository : Repository<RefundOrder>, IRefundOrderRepository
    {
        public RefundOrderRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo)
        {
            return DbSet.AsNoTracking().Any(c => c.MchNo.Equals(mchNo) && c.MchRefundNo.Equals(mchRefundNo));
        }

        public bool IsExistRefundingOrder(string payOrderId)
        {
            return DbSet.AsNoTracking().Any(c => c.PayOrderId.Equals(payOrderId) && c.State.Equals((byte)RefundOrderState.STATE_ING));
        }
    }
}
