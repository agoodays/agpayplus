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

        public override bool Add(SysUserAuthDto dto)
        {
            var m = _mapper.Map<SysUserAuth>(dto);
            _sysUserAuthRepository.Add(m);
            var result = _sysUserAuthRepository.SaveChanges(out int _);
            dto.AuthId = m.AuthId;
            return result;
        }

        public SysUserAuthDto GetByIdentifier(byte identityType, string identifier, string sysType)
        {
            var entity = _sysUserAuthRepository.GetByIdentifier(identityType, identifier, sysType);
            var dto = _mapper.Map<SysUserAuthDto>(entity);
            return dto;
        }

        public void ResetAuthInfo(long resetUserId, string authLoginUserName, string telphone, string newPwd, string sysType)
        {
            _sysUserAuthRepository.ResetAuthInfo(resetUserId, sysType, authLoginUserName, telphone, newPwd);
            _sysUserAuthRepository.SaveChanges();
        }
    }
}
