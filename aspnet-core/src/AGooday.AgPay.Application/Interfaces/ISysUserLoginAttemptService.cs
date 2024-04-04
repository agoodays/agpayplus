using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserLoginAttemptService : IAgPayService<SysUserLoginAttemptDto, long>
    {
        Task ClearFailedLoginAttemptsAsync(long userId);
        Task<(int failedAttempts, DateTime? lastLoginTime)> GetFailedLoginAttemptsAsync(long userId, TimeSpan timeWindow);
        Task<DateTime?> GetLastLoginTimeAsync(long userId);
        Task RecordLoginAttemptAsync(SysUserLoginAttemptDto dto);
    }
}
