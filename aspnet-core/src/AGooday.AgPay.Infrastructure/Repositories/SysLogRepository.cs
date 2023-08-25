using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysLogRepository : Repository<SysLog, long>, ISysLogRepository
    {
        public SysLogRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public SysLog GetLastSysLog(long userId, string methodRemark, string sysType)
        {
            return DbSet.OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault(w => w.UserId.Equals(userId) && w.MethodRemark.Equals(methodRemark) && w.SysType.Equals(sysType));
        }
    }
}
