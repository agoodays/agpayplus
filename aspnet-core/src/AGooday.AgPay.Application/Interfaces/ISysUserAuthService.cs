using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserAuthService : IDisposable
    {
        void Add(SysUserAuthDto dto);
        void Remove(long recordId);
        void Update(SysUserAuthDto dto);
        SysUserAuthDto GetById(long recordId);
        SysUserAuthDto GetByIdentifier(byte identityType, string identifier, string sysType);
        IEnumerable<SysUserAuthDto> GetAll();
        void ResetAuthInfo(long resetUserId, string authLoginUserName, string telphone, string newPwd, string sysType);
    }
}
