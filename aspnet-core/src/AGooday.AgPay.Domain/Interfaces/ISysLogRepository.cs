using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysLogRepository : IRepository<SysLog, long>
    {
        SysLog GetLastSysLog(long userId, string methodRemark, string sysType);
    }
}
