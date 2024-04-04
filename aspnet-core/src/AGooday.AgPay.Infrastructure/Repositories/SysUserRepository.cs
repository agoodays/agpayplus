using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserRepository : AgPayRepository<SysUser, long>, ISysUserRepository
    {
        public SysUserRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistLoginUsername(string loginUsername, string sysType)
        {
            return GetAllAsNoTracking().Any(c => c.LoginUsername.Equals(loginUsername) && c.SysType.Equals(sysType));
        }

        public bool IsExistTelphone(string telphone, string sysType)
        {
            return GetAllAsNoTracking().Any(c => c.Telphone.Equals(telphone) && c.SysType.Equals(sysType));
        }

        public bool IsExistUserNo(string userNo, string sysType)
        {
            return GetAllAsNoTracking().Any(c => c.UserNo.Equals(userNo) && c.SysType.Equals(sysType));
        }

        public bool IsExist(long sysUserId, string sysType)
        {
            return GetAllAsNoTracking().Any(c => c.SysUserId.Equals(sysUserId) && c.SysType.Equals(sysType));
        }

        public SysUser GetByKeyAsNoTracking(long recordId)
        {
            return GetAllAsNoTracking().FirstOrDefault(w => w.SysUserId.Equals(recordId));
        }

        public IQueryable<SysUser> GetByBelongInfoIdAsNoTracking(string belongInfoId)
        {
            return GetAllAsNoTracking().Where(w => w.BelongInfoId.Equals(belongInfoId));
        }

        public SysUser GetByUserId(long sysUserId)
        {
            return DbSet.Single(w => w.SysUserId.Equals(sysUserId));
        }

        public SysUser GetByUserId(long sysUserId, string sysType)
        {
            return DbSet.FirstOrDefault(w => w.SysUserId.Equals(sysUserId) && w.SysType.Equals(sysType));
        }

        public SysUser GetByTelphone(string telphone, string sysType)
        {
            return DbSet.FirstOrDefault(w => w.Telphone.Equals(telphone) && w.SysType.Equals(sysType));
        }

        public long FindMchAdminUserId(string mchNo)
        {
            return DbSet.First(w => w.BelongInfoId.Equals(mchNo) && w.SysType.Equals(CS.SYS_TYPE.MCH) && w.UserType.Equals(CS.USER_TYPE.ADMIN)).SysUserId;
        }

        public long FindAgentAdminUserId(string agentNo)
        {
            return DbSet.First(w => w.BelongInfoId.Equals(agentNo) && w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.UserType.Equals(CS.USER_TYPE.ADMIN)).SysUserId;
        }

        public void Remove(SysUser sysUser)
        {
            DbSet.Remove(sysUser);
        }
    }
}
