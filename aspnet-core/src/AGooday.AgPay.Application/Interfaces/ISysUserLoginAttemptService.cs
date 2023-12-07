using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserLoginAttemptService : IDisposable
    {
        Task ClearFailedLoginAttemptsAsync(long userId);
        Task<(int failedAttempts, DateTime? lastLoginTime)> GetFailedLoginAttemptsAsync(long userId, TimeSpan timeWindow);
        Task RecordLoginAttemptAsync(SysUserLoginAttemptDto dto);
    }
}
