using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 操作员认证表 服务实现类
    /// </summary>
    public class SysUserAuthService : AgPayService<SysUserAuthDto, SysUserAuth, long>, ISysUserAuthService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserAuthRepository _sysUserAuthRepository;

        public SysUserAuthService(IMapper mapper, IMediatorHandler bus,
        ISysUserAuthRepository sysUserAuthRepository)
            : base(mapper, bus, sysUserAuthRepository)
        {
            _sysUserAuthRepository = sysUserAuthRepository;
        }

        public override async Task<bool> AddAsync(SysUserAuthDto dto)
        {
            var entity = _mapper.Map<SysUserAuth>(dto);
            await _sysUserAuthRepository.AddAsync(entity);
            var (result, _) = await _sysUserAuthRepository.SaveChangesWithResultAsync();
            dto.AuthId = entity.AuthId;
            return result;
        }

        public async Task<SysUserAuthDto> GetByIdentifierAsync(byte identityType, string identifier, string sysType)
        {
            var entity = await _sysUserAuthRepository.GetByIdentifierAsync(identityType, identifier, sysType);
            var dto = _mapper.Map<SysUserAuthDto>(entity);
            return dto;
        }

        public async Task ResetAuthInfoAsync(long resetUserId, string authLoginUserName, string telphone, string newPwd, string sysType)
        {
            await _sysUserAuthRepository.ResetAuthInfoAsync(resetUserId, sysType, authLoginUserName, telphone, newPwd);
            await _sysUserAuthRepository.SaveChangesAsync();
        }
    }
}
