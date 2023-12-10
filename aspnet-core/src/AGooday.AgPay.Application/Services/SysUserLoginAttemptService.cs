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
    public class SysUserLoginAttemptService : ISysUserLoginAttemptService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserLoginAttemptRepository _sysUserLoginAttemptRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserLoginAttemptService(IMapper mapper, IMediatorHandler bus,
            ISysUserLoginAttemptRepository sysUserLoginAttemptRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysUserLoginAttemptRepository = sysUserLoginAttemptRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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

        public async Task ClearFailedLoginAttemptsAsync(long userId)
        {
            await _sysUserLoginAttemptRepository.ClearFailedLoginAttemptsAsync(userId);
            await _sysUserLoginAttemptRepository.SaveChangesAsync();
        }
    }
}
