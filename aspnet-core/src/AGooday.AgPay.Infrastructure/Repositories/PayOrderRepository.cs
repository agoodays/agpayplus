using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayOrderRepository : Repository<PayOrder>, IPayOrderRepository
    {
        public PayOrderRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistOrderUseIfCode(string ifCode)
        {
            return DbSet.AsNoTracking().Any(c => c.IfCode.Equals(ifCode));
        }

        public bool IsExistOrderUseMchNo(string mchNo)
        {
            return DbSet.AsNoTracking().Any(c => c.MchNo.Equals(mchNo));
        }

        public bool IsExistOrderUseWayCode(string wayCode)
        {
            return DbSet.AsNoTracking().Any(c => c.WayCode.Equals(wayCode));
        }

        public bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo)
        {
            return DbSet.AsNoTracking().Any(c => c.MchNo.Equals(mchNo) && c.MchOrderNo.Equals(mchOrderNo));
        }
    }
}
