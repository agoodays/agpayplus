using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserService : IAgPayService<SysUserDto, long>
    {
        Task CreateAsync(SysUserCreateDto dto);
        Task RemoveAsync(long sysUserId, long currentUserId, string sysType);
        Task ModifyCurrentUserInfoAsync(ModifyCurrentUserInfoDto user);
        Task ModifyAsync(SysUserModifyDto dto);
        Task<SysUserDto> GetByKeyAsNoTrackingAsync(long recordId);
        IEnumerable<SysUserDto> GetByBelongInfoIdAsNoTracking(string belongInfoId);
        Task<SysUserDto> GetByIdAsync(long recordId, string belongInfoId);
        Task<bool> IsExistTelphoneAsync(string telphone, string sysType);
        Task<SysUserDto> GetByTelphoneAsync(string telphone, string sysType);
        IEnumerable<SysUserDto> GetByIds(List<long> recordIds);
        PaginatedList<SysUserListDto> GetPaginatedData(SysUserQueryDto dto, long? currentUserId);
        Task<PaginatedList<SysUserListDto>> GetPaginatedDataAsync(SysUserQueryDto dto, long? currentUserId);
    }
}
