using AGooday.AgPay.Application.DataTransfer;

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
        SysUserDto GetByUserId(long sysUserId);
        SysUserDto GetById(long recordId, string belongInfoId);
        IEnumerable<SysUserDto> GetAll();
        IEnumerable<SysUserDto> GetAll(List<long> recordIds);
        PaginatedList<SysUserListDto> GetPaginatedData(SysUserQueryDto dto, long currentUserId);
        Task<PaginatedList<SysUserListDto>> GetPaginatedDataAsync(SysUserQueryDto dto, long currentUserId);
        Task<IEnumerable<SysUserDto>> ListAsync();
    }
}
