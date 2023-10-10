using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserTeamService : IDisposable
    {
        bool Add(SysUserTeamDto dto);
        bool Remove(long recordId);
        bool Update(SysUserTeamDto dto);
        SysUserTeamDto GetById(long recordId);
        IEnumerable<SysUserTeamDto> GetAll();
        PaginatedList<SysUserTeamDto> GetPaginatedData(SysUserTeamQueryDto dto);
    }
}
