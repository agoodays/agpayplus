using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysUserLoginAttemptRepository : Repository<SysUserLoginAttempt, long>, ISysUserLoginAttemptRepository
    {
        public SysUserLoginAttemptRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public async Task RecordFailedLoginAttempt(long userId, string identifier, byte identityType, string ipAddress, string sysType)
        {
            var loginAttempt = new SysUserLoginAttempt
            {
                UserId = userId,
                IdentityType = identityType,
                Identifier = identifier,
                IpAddress = ipAddress,
                SysType = sysType,
                AttemptTime = DateTime.Now,
                Success = false
            };

            // 将登录尝试记录保存到数据库
            await DbSet.AddAsync(loginAttempt);
        }

        public async Task<(int failedAttempts, DateTime? lastLoginTime)> GetFailedLoginAttemptsAsync(long userId, TimeSpan timeWindow)
        {
            DateTime startTime = DateTime.Now - timeWindow;

            int failedAttempts = await DbSet.CountAsync(w => w.UserId.Equals(userId) && !w.Success && w.AttemptTime >= startTime);

            DateTime? lastLoginTime = await DbSet
                .Where(w => w.UserId.Equals(userId) && w.Success)
                .OrderByDescending(l => l.AttemptTime)
                .Select(l => l.AttemptTime)
                .FirstOrDefaultAsync();

            return (failedAttempts, lastLoginTime);
        }

        public async Task ClearFailedLoginAttemptsAsync(long userId)
        {
            var loginAttempts = await DbSet
                .Where(w => w.UserId == userId && !w.Success)
                .ToListAsync();

            DbSet.RemoveRange(loginAttempts);
        }
    }
}
