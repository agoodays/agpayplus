using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统操作员表 服务实现类
    /// </summary>
    public class SysUserLoginAttemptService : AgPayService<SysUserLoginAttemptDto, SysUserLoginAttempt, long>, ISysUserLoginAttemptService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserLoginAttemptRepository _sysUserLoginAttemptRepository;

        public SysUserLoginAttemptService(IMapper mapper, IMediatorHandler bus,
            ISysUserLoginAttemptRepository sysUserLoginAttemptRepository)
            : base(mapper, bus, sysUserLoginAttemptRepository)
        {
            _sysUserLoginAttemptRepository = sysUserLoginAttemptRepository;
        }

        public async Task RecordLoginAttemptAsync(SysUserLoginAttemptDto dto)
        {
            var entity = _mapper.Map<SysUserLoginAttempt>(dto);
            await _sysUserLoginAttemptRepository.AddAsync(entity);
            await _sysUserLoginAttemptRepository.SaveChangesAsync();
            dto.Id = entity.Id;
        }

        public async Task<(int failedAttempts, DateTime? lastLoginTime)> GetFailedLoginAttemptsAsync(long userId, TimeSpan timeWindow)
        {
            return await _sysUserLoginAttemptRepository.GetFailedLoginAttemptsAsync(userId, timeWindow);
        }

        public async Task<DateTime?> GetLastLoginTimeAsync(long userId)
        {
            return await _sysUserLoginAttemptRepository.GetLastLoginTimeAsync(userId);
        }

        public async Task ClearFailedLoginAttemptsAsync(long userId)
        {
            await _sysUserLoginAttemptRepository.ClearFailedLoginAttemptsAsync(userId);
            await _sysUserLoginAttemptRepository.SaveChangesAsync();
        }
    }
}
