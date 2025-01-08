using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserRepository : IAgPayRepository<SysUser, long>
    {
        Task<bool> IsExistLoginUsernameAsync(string loginUsername, string sysType);
        Task<bool> IsExistTelphoneAsync(string telphone, string sysType);
        Task<bool> IsExistUserNoAsync(string userNo, string sysType);
        Task<bool> IsExistInviteCodeAsync(string inviteCode);
        Task<bool> IsExistAsync(long sysUserId, string sysType);
        Task<SysUser> GetByKeyAsNoTrackingAsync(long recordId);
        IQueryable<SysUser> GetByBelongInfoIdAsNoTracking(string belongInfoId);
        Task<SysUser> GetByUserIdAsync(long sysUserId);
        Task<SysUser> GetByUserIdAsync(long sysUserId, string sysType);
        Task<SysUser> GetByTelphoneAsync(string telphone, string sysType);
        Task<long> FindMchAdminUserIdAsync(string mchNo);
        Task<long> FindAgentAdminUserIdAsync(string agentNo);
    }
}
