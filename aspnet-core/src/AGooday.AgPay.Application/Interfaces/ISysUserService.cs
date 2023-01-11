using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserService : IDisposable
    {
        void Add(SysUserDto dto);
        void Create(SysUserCreateDto dto);
        void Remove(long recordId);
        void Remove(long sysUserId, long currentUserId, string sysType);
        void Update(SysUserDto dto);
        void ModifyCurrentUserInfo(ModifyCurrentUserInfoDto user);
        void Modify(SysUserModifyDto dto);
        SysUserDto GetById(long recordId);
        SysUserDto GetByUserId(long sysUserId);
        SysUserDto GetById(long recordId, string belongInfoId);
        IEnumerable<SysUserDto> GetAll();
        IEnumerable<SysUserDto> GetAll(List<long> recordIds);
        PaginatedList<SysUserListDto> GetPaginatedData(SysUserQueryDto dto, long currentUserId);
        Task<IEnumerable<SysUserDto>> ListAsync();
    }
}
