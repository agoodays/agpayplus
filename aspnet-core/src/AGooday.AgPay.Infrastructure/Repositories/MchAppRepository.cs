using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchAppRepository : Repository<MchApp>, IMchAppRepository
    {
        public MchAppRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public MchApp GetAsNoTrackingById(string id)
        {
            return DbSet.AsNoTracking().Where(w => w.AppId.Equals(id)).FirstOrDefault();
        }
    }
}
