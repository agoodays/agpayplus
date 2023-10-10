using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserService : IDisposable
    {
        void Add(SysUserDto dto);
        Task Create(SysUserCreateDto dto);
        void Remove(long recordId);
        Task Remove(long sysUserId, long currentUserId, string sysType);
        void Update(SysUserDto dto);
        void ModifyCurrentUserInfo(ModifyCurrentUserInfoDto user);
        Task Modify(SysUserModifyDto dto);
        SysUserDto GetByKeyAsNoTracking(long recordId);
        SysUserDto GetById(long recordId);
        Task<SysUserDto> GetByIdAsync(long recordId);
        SysUserDto GetById(long recordId, string belongInfoId);
        bool IsExistTelphone(string telphone, string sysType);
        SysUserDto GetByTelphone(string telphone, string sysType);
        IEnumerable<SysUserDto> GetAll();
        IEnumerable<SysUserDto> GetByIds(List<long> recordIds);
        PaginatedList<SysUserListDto> GetPaginatedData(SysUserQueryDto dto, long currentUserId);
        Task<PaginatedList<SysUserListDto>> GetPaginatedDataAsync(SysUserQueryDto dto, long currentUserId);
        Task<IEnumerable<SysUserDto>> ListAsync();
    }
}
