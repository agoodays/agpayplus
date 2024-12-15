using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class RefundOrderRepository : AgPayRepository<RefundOrder>, IRefundOrderRepository
    {
        public RefundOrderRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchRefundNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.MchNo.Equals(mchNo) && c.MchRefundNo.Equals(mchRefundNo));
        }

        public Task<bool> IsExistRefundingOrderAsync(string payOrderId)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.PayOrderId.Equals(payOrderId) && c.State.Equals((byte)RefundOrderState.STATE_ING));
        }
    }
}
