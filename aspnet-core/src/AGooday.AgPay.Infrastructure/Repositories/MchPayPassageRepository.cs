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
    public class MchPayPassageRepository : Repository<MchPayPassage, long>, IMchPayPassageRepository
    {
        public MchPayPassageRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistMchPayPassageUseWayCode(string wayCode)
        {
            return DbSet.AsNoTracking().Any(c => c.WayCode.Equals(wayCode));
        }

        public void RemoveByMchNo(string mchNo)
        {
            var mchPayPassages = DbSet.Where(w => w.MchNo.Equals(mchNo));
            foreach (var mchPayPassage in mchPayPassages)
            {
                Remove(mchPayPassage.Id);
            }
        }
    }
}
