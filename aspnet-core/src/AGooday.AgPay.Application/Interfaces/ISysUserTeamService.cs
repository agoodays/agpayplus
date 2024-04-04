using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserTeamService : IAgPayService<SysUserTeamDto>
    {
        PaginatedList<SysUserTeamDto> GetPaginatedData(SysUserTeamQueryDto dto);
    }
}
