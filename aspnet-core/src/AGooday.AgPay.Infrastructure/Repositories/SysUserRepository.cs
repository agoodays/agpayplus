using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserRepository : AgPayRepository<SysUser, long>, ISysUserRepository
    {
        public SysUserRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistLoginUsernameAsync(string loginUsername, string sysType)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.LoginUsername.Equals(loginUsername) && c.SysType.Equals(sysType));
        }

        public Task<bool> IsExistTelphoneAsync(string telphone, string sysType)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.Telphone.Equals(telphone) && c.SysType.Equals(sysType));
        }

        public Task<bool> IsExistUserNoAsync(string userNo, string sysType)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.UserNo.Equals(userNo) && c.SysType.Equals(sysType));
        }

        public Task<bool> IsExistInviteCodeAsync(string inviteCode)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.InviteCode.Equals(inviteCode));
        }

        public Task<bool> IsExistAsync(long sysUserId, string sysType)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.SysUserId.Equals(sysUserId) && c.SysType.Equals(sysType));
        }

        public Task<SysUser> GetByKeyAsNoTrackingAsync(long recordId)
        {
            return GetAllAsNoTracking().FirstOrDefaultAsync(w => w.SysUserId.Equals(recordId));
        }

        public IQueryable<SysUser> GetByBelongInfoIdAsNoTracking(string belongInfoId)
        {
            return GetAllAsNoTracking().Where(w => w.BelongInfoId.Equals(belongInfoId));
        }

        public Task<SysUser> GetByUserIdAsync(long sysUserId)
        {
            return DbSet.SingleAsync(w => w.SysUserId.Equals(sysUserId));
        }

        public Task<SysUser> GetByUserIdAsync(long sysUserId, string sysType)
        {
            return DbSet.FirstOrDefaultAsync(w => w.SysUserId.Equals(sysUserId) && w.SysType.Equals(sysType));
        }

        public Task<SysUser> GetByTelphoneAsync(string telphone, string sysType)
        {
            return DbSet.FirstOrDefaultAsync(w => w.Telphone.Equals(telphone) && w.SysType.Equals(sysType));
        }

        public async Task<long> FindMchAdminUserIdAsync(string mchNo)
        {
            return (await DbSet.FirstAsync(w => w.BelongInfoId.Equals(mchNo) && w.SysType.Equals(CS.SYS_TYPE.MCH) && w.UserType.Equals(CS.USER_TYPE.ADMIN))).SysUserId;
        }

        public async Task<long> FindAgentAdminUserIdAsync(string agentNo)
        {
            return (await DbSet.FirstAsync(w => w.BelongInfoId.Equals(agentNo) && w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.UserType.Equals(CS.USER_TYPE.ADMIN))).SysUserId;
        }
    }
}
