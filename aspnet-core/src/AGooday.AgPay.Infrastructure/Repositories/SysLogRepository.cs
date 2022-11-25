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
    }
}
