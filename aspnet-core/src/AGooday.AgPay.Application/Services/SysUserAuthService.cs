using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 操作员认证表 服务实现类
    /// </summary>
    public class SysUserAuthService : ISysUserAuthService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserAuthService(IMapper mapper, IMediatorHandler bus)
        {
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysUserAuthDto dto)
        {
            var m = _mapper.Map<SysUserAuth>(dto);
            _sysUserAuthRepository.Add(m);
            _sysUserAuthRepository.SaveChanges();
            dto.AuthId = m.AuthId;
        }

        public void Remove(long recordId)
        {
            _sysUserAuthRepository.Remove(recordId);
            _sysUserAuthRepository.SaveChanges();
        }

        public void Update(SysUserAuthDto dto)
        {
            var m = _mapper.Map<SysUserAuth>(dto);
            _sysUserAuthRepository.Update(m);
            _sysUserAuthRepository.SaveChanges();
        }

        public SysUserAuthDto GetById(long recordId)
        {
            var entity = _sysUserAuthRepository.GetById(recordId);
            var dto = _mapper.Map<SysUserAuthDto>(entity);
            return dto;
        }

        public SysUserAuthDto GetByIdentifier(byte identityType, string identifier, string sysType)
        {
            var entity = _sysUserAuthRepository.GetByIdentifier(identityType, identifier, sysType);
            var dto = _mapper.Map<SysUserAuthDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserAuthDto> GetAll()
        {
            var sysUserAuths = _sysUserAuthRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserAuthDto>>(sysUserAuths);
        }

        public void ResetAuthInfo(long resetUserId, string authLoginUserName, string telphone, string newPwd, string sysType)
        {
            _sysUserAuthRepository.ResetAuthInfo(resetUserId, sysType, authLoginUserName, telphone, newPwd);
            _sysUserAuthRepository.SaveChanges();
        }
    }
}
