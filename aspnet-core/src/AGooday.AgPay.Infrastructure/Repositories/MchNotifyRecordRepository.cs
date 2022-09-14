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
    public class MchNotifyRecordRepository : Repository<MchNotifyRecord, long>, IMchNotifyRecordRepository
    {
        public MchNotifyRecordRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
