using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysUserLoginAttemptRepository : IRepository<SysUserLoginAttempt, long>
    {
        Task<(int failedAttempts, DateTime? lastLoginTime)> GetFailedLoginAttemptsAsync(long userId, TimeSpan timeWindow);
        Task<DateTime?> GetLastLoginTimeAsync(long userId);
        Task ClearFailedLoginAttemptsAsync(long userId);
    }
}
