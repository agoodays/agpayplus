using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysLogService : IDisposable
    {
        void Add(SysLogDto dto);
        void Remove(long recordId);
        bool RemoveByIds(List<long> recordIds);
        void Update(SysLogDto dto);
        SysLogDto GetById(long recordId);
        SysLogDto GetLastSysLog(long userId, string methodRemark, string sysType);
        IEnumerable<SysLogDto> GetAll();
        PaginatedList<SysLogDto> GetPaginatedData(SysLogQueryDto dto);
    }
}
