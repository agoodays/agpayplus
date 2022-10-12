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
    }
}
