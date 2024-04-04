using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayOrderRepository : AgPayRepository<PayOrder>, IPayOrderRepository
    {
        public PayOrderRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistOrderUseIfCode(string ifCode)
        {
            return GetAllAsNoTracking().Any(c => c.IfCode.Equals(ifCode));
        }

        public bool IsExistOrderUseMchNo(string mchNo)
        {
            return GetAllAsNoTracking().Any(c => c.MchNo.Equals(mchNo));
        }

        public bool IsExistOrderUseWayCode(string wayCode)
        {
            return GetAllAsNoTracking().Any(c => c.WayCode.Equals(wayCode));
        }

        public bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo)
        {
            return GetAllAsNoTracking().Any(c => c.MchNo.Equals(mchNo) && c.MchOrderNo.Equals(mchOrderNo));
        }
    }
}
