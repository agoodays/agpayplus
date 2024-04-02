using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserAuthRepository : IAgPayRepository<SysUserAuth, long>
    {
        void RemoveByUserId(long userId, string sysType);
        void ResetAuthInfo(long userId, string sysType, string loginUserName, string telphone, string newPwd);
        SysUserAuth GetByIdentifier(byte identityType, string identifier, string sysType);
        List<SysUserAuth> GetUserAuths(string identifier, string sysType);
    }
}
