using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserRepository : IRepository<SysUser, long>
    {
        bool IsExistLoginUsername(string loginUsername, string sysType);
        bool IsExistTelphone(string telphone, string sysType);
        bool IsExistUserNo(string userNo, string sysType);
        bool IsExist(long sysUserId, string sysType);
        SysUser GetByKeyAsNoTracking(long recordId);
        SysUser GetByUserId(long sysUserId);
        SysUser GetByUserId(long sysUserId, string sysType);
        SysUser GetByTelphone(string telphone, string sysType);
        void Remove(SysUser sysUser);
        long FindMchAdminUserId(string mchNo);
        long FindAgentAdminUserId(string agentNo);
    }
}
