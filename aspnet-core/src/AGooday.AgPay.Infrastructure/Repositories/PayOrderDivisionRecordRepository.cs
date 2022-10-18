using AGooday.AgPay.Common.Enumerator;
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
    public class PayOrderDivisionRecordRepository : Repository<PayOrderDivisionRecord, long>, IPayOrderDivisionRecordRepository
    {
        public PayOrderDivisionRecordRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public long SumSuccessDivisionAmount(string payOrderId)
        {
            return DbSet.Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)PayOrderDivisionState.STATE_SUCCESS))
                .Sum(s => s.CalDivisionAmount);
        }
    }
}
